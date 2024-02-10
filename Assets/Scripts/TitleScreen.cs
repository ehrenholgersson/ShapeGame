using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] Material _worldMaterial;
    Color _levelColor = Color.grey;
    [SerializeField] float _maxColTransitionTime;
    [SerializeField] float _minColTransitionTime;
    [SerializeField] float _maxColHoldTime;
    [SerializeField] float _minColHoldTime;
    [SerializeField] Color _deadColor;
    [SerializeField] List<Color> _worldcolors = new List<Color>();

    static TitleScreen _instance;

    public static Color LevelColor { get => _instance._levelColor; }

    private void Awake()
    {
        Application.targetFrameRate = 120;
        _instance = this;
    }

    private void Start()
    {
        _levelColor = _worldcolors[0];
        _worldMaterial.SetColor("_Color", _levelColor);
        //_windMaterial.SetColor("_TintColor", _levelColor);
        StartCoroutine(ColorChanger());
    }

    IEnumerator ColorChanger()
    {
        if (!_worldMaterial.HasProperty("_Color"))
        {
            Debug.Log("Material has no _Color property");
            yield break;
        }
        int nextColor = 1;//first colour should be 0 so the "next" colour is 1
        float colorTime = Time.time;
        float hold;
        float transition;
        Color oldColor;

        if (_worldcolors.Count > 1) // no point if 1 or fewer colours
        {
            while (_instance != null)
            {
                    hold = UnityEngine.Random.Range(_minColHoldTime, _maxColHoldTime);
                    transition = UnityEngine.Random.Range(_minColTransitionTime, _maxColTransitionTime);
                    while ((Time.time < colorTime + hold) && _instance != null)
                    {
                        yield return new WaitForFixedUpdate();
                    }
                    colorTime = Time.time;
                    oldColor = _levelColor;
                    Debug.Log("Change color");
                    while ((Time.time < colorTime + transition) && _instance != null)
                    {
                        _levelColor = Color.Lerp(oldColor, _worldcolors[nextColor], (Time.time - colorTime) / transition);
                        // set material colours
                        _worldMaterial.SetColor("_Color", _levelColor);
                        // _windMaterial.SetColor("_TintColor", _levelColor);

                        yield return new WaitForFixedUpdate();
                    }
                    colorTime = Time.time;
                    nextColor = UnityEngine.Random.Range(0, _worldcolors.Count);

                yield return null;
            }
        }
    }
}
