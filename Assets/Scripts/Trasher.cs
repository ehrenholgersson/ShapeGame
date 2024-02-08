using UnityEngine;

// Script sits on a collider left of screen to remove terrain pieces after they have moved out of view
// Spawns a new random piece of terrain every time one is removed
// To Do -
// * pool terrain pieces.
// * Terrain should be grouped by difficulty, with the more difficult obstacles spawning more frequently over time
public class Trasher : MonoBehaviour 
{
    private float _terrainLength;
    private static Trasher _instance;

    //public static float TerrainLength { get => _instance?._terrainLength ?? -1; set => _instance._terrainLength = value; }

    private void Awake()
    {
        _instance = this;

    }

    private void Start()
    {
        // check length of terrain pieces, this is a double up as the pieces will also be adding themselves but as both run on awake and _instance will not be set initially we will set to 0 and go again on start.
        //removed this for now as pieces are using OnEnable to add their length, which should run after trasher instance is initialized
        //_terrainLength = 0;
        //foreach (GameObject terrain in GameObject.FindGameObjectsWithTag("Reset"))
        //{
        //    _terrainLength += terrain.GetComponent<TerrainMovement>()?.Length ?? 0;
        //}
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Trash collider has an object");
        if (collision.gameObject.name.Contains("World"))
        {
            collision.gameObject.SetActive(false);
            SpawnNew();
        }
        else if (collision.transform.parent.name.Contains("World"))
        {
            collision.transform.parent.gameObject.SetActive(false);
            SpawnNew();
        }
    }
    private void SpawnNew()
    {
        GameControl.Instance.SpawnTerrain();
    }

    private void stupidtest(Rigidbody2D PlayerRB)
    {
        if (PlayerRB.velocity.x > 20 || PlayerRB.velocity.x < -20)
            PlayerRB.velocity = new Vector2((PlayerRB.velocity.x/Mathf.Abs(PlayerRB.velocity.x)*20), PlayerRB.velocity.y);
    }
}


