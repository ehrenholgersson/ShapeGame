using UnityEngine;

public class Restart : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameControl.Instance.Restart();
        }
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
#endif
    }

}
