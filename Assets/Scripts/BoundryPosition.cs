using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundryPosition : MonoBehaviour
{
    enum Side {Left, Right }
    [SerializeField] Side _side;
    // Start is called before the first frame update
    void Start()
    {
        if (_side == Side.Left)
        {
            transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Vector3.zero).x - 3, transform.position.y, transform.position.z);
        }
    }

}
