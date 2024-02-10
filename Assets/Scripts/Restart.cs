using UnityEngine;

public class Restart : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
#if UNITY_ANDROID
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended) 
        {
            GameControl.Instance.Restart();
        }
#endif
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
