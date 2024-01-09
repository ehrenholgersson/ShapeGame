using UnityEngine;

public class Trasher : MonoBehaviour
{
    public GameObject lastSpawned;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trash collider has an object");
        if (collision.gameObject.name.Contains("World"))
        {
            Destroy(collision.gameObject);
            SpawnNew();
        }
        else if (collision.transform.parent.name.Contains("World"))
        {
            Destroy(collision.transform.parent.gameObject);
            SpawnNew();
        }
    }
    private void SpawnNew()
    {
        int rng = Random.Range(1, GameControl.Instance._terrainList.pieces.Count);
        GameObject go = Instantiate(GameControl.Instance._terrainList.pieces[rng]);
        go.transform.position = lastSpawned.transform.position + new Vector3(30,0,0);
        lastSpawned = go;
    }

    private void stupidtest(Rigidbody2D PlayerRB)
    {
        if (PlayerRB.velocity.x > 20 || PlayerRB.velocity.x < -20)
            PlayerRB.velocity = new Vector2((PlayerRB.velocity.x/Mathf.Abs(PlayerRB.velocity.x)*20), PlayerRB.velocity.y);
    }
}


