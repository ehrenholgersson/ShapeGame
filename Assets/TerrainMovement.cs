using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMovement : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.position -= new Vector3(GameControl.Instance.worldSpeed, 0, 0) * Time.deltaTime;
    }
}
