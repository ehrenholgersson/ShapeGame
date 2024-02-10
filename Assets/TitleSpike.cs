using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSpike : MonoBehaviour
{
    Rigidbody2D _rb;
    [SerializeField] float _lifeTime;
    [SerializeField] float _minSpawnDelay;
    [SerializeField] float _maxSpawnDelay;
    float _spawnDelay;
    float _spawnTime;
    Camera _cam;


    // Start is called before the first frame update

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _cam = Camera.main;
    }
    void Start()
    {
        _rb.velocity = new Vector3(-10, 0, 0);
        _spawnTime = Time.time;
        _spawnDelay = Random.Range(_minSpawnDelay, _maxSpawnDelay);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.time - _spawnTime > _lifeTime + _spawnDelay)
        {
            transform.position = _cam.ScreenToWorldPoint(new Vector3(_cam.pixelWidth, Random.Range(0,_cam.pixelHeight))) + new Vector3(5,0,0);
            _rb.velocity = new Vector3( - 10, 0, 0);
            _spawnTime = Time.time;
            _spawnDelay = Random.Range(_minSpawnDelay, _maxSpawnDelay);
        }
    }
}
