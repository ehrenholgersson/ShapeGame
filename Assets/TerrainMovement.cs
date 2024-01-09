using UnityEngine;

public class TerrainMovement : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.position -= new Vector3(GameControl.Instance.WorldSpeed, 0, 0) * Time.deltaTime;
    }
}
