using UnityEngine;

// Script sits on a collider left of screen to remove terrain pieces after they have moved out of view
// Spawns a new random piece of terrain every time one is removed
// To Do -
// * pool terrain pieces.
// * Terrain should be grouped by difficulty, with the more difficult obstacles spawning more frequently over time
public class Trasher : MonoBehaviour 
{
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


