using System.Collections.Generic;
using UnityEngine;

public class TerrainMovement : MonoBehaviour
{
    [SerializeField] float _length = 30;
    public float Length { get => _length; }
    List<GameObject> _pool;

    private void Awake()
    {
        _length = transform.Find("Bounds")?.GetComponent<Collider2D>()?.bounds.size.x ?? 30;
    }
    public void SetPool(List<GameObject> pool) => _pool = pool;
    private void OnEnable()
    {
        GameControl.LastSpawnedSize = _length;
        if (_pool != null && _pool.Contains(gameObject)) 
        {
            _pool.Remove(gameObject);
        }
        if (GameControl.LastSpawned != null)
        {
            transform.position = new Vector3(GameControl.LastSpawned.transform.position.x + ((GameControl.LastSpawnedSize + _length) / 2), transform.position.y, transform.position.z);
        }
        else
        {
            transform.position = GameControl.FirstTerrainPos;
        }
        //Trasher.TerrainLength += _length;
    }

    private void OnDisable()
    {
        //Trasher.TerrainLength -= _length;
        if (_pool != null && !_pool.Contains(gameObject)) 
        {
            _pool.Add(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position -= new Vector3(GameControl.Instance.WorldSpeed, 0, 0) * Time.deltaTime;
    }
}
