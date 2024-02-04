using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class GameControl : MonoBehaviour
{
    #region Variables

    static GameControl _instance;
    float _timer = 0;
    
    List<TextMeshProUGUI> _timeReadouts = new List<TextMeshProUGUI>();
    [SerializeField] ParticleSystem _windParticles;
    [SerializeField] GameObject _gameOver;
    [SerializeField] GameObject _player;
    [SerializeField] public TerrainList _terrainList; // check if this needs to be private or a function
    [SerializeField] float _worldSpeed;

    [SerializeField] Material _worldMaterial;
    //[SerializeField] Material _windMaterial;
    Color _levelColor = Color.grey;
    [SerializeField] float _maxColTransitionTime;
    [SerializeField] float _minColTransitionTime;
    [SerializeField] float _maxColHoldTime;
    [SerializeField] float _minColHoldTime;
    [SerializeField] Color _deadColor;
    [SerializeField] List<Color> _worldcolors = new List<Color>();

    #endregion

    #region Properties

    public static GameControl Instance { get => _instance ?? null; }
    public GameObject GameOver { get => _gameOver ?? null; }
    public GameObject Player { get => _player ?? null; }
    public float WorldSpeed { get => _worldSpeed; }
    public static float RunTimer { get => _instance._timer; }
    public static Color LevelColor { get => _instance._levelColor; }

    #endregion

    #region Initialization
    // Start is called before the first frame update
    private void Awake()
    {
        // setup platform specific elements
        #region Platform Specific Setup 

#if UNITY_WEBGL
        Debug.Log("Running WebGL");
        foreach (GameObject ui in GameObject.FindGameObjectsWithTag("WebUI"))
        {
            Debug.Log("found UI object " + ui.name);
            if (ui.name.Contains("GameOver"))
            {
                _gameOver = ui;
                ui.SetActive(false);
            }
            else if (ui.name.Contains("Timer"))
            {
                if (ui.TryGetComponent<TextMeshProUGUI>(out TextMeshProUGUI tmp))
                {
                    _timeReadouts.Add(tmp);
                }
            }


        }
        foreach (GameObject ui in GameObject.FindGameObjectsWithTag("DesktopUI"))
        {
            ui.SetActive(false);
        }
        foreach (GameObject ui in GameObject.FindGameObjectsWithTag("MobileUI"))
        {
            ui.SetActive(false);
        }
#endif
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
        Debug.Log("Running Desktop");
        foreach (GameObject ui in GameObject.FindGameObjectsWithTag("DesktopUI"))
        {
            Debug.Log("found UI object " + ui.name);
            if (ui.name.Contains("GameOver"))
            {
                _gameOver = ui;
            }
            ui.SetActive(false);
        }
        foreach (GameObject ui in GameObject.FindGameObjectsWithTag("WebUI"))
        {
            ui.SetActive(false);
        }
        foreach (GameObject ui in GameObject.FindGameObjectsWithTag("MobileUI"))
        {
            ui.SetActive(false);
        }
#endif
#if UNITY_ANDROID
        Debug.Log("Running Mobile");
        foreach (GameObject ui in GameObject.FindGameObjectsWithTag("MobileUI"))
        {
            Debug.Log("found UI object " + ui.name);
            if (ui.name.Contains("GameOver"))
            {
                _gameOver = ui;
            }
            ui.SetActive(false);
        }
        foreach (GameObject ui in GameObject.FindGameObjectsWithTag("WebUI"))
        {
            ui.SetActive(false);
        }
        foreach (GameObject ui in GameObject.FindGameObjectsWithTag("DesktopUI"))
        {
            ui.SetActive(false);
        }
#endif
        #endregion

        Application.targetFrameRate = 120;
        _instance = this;
    }
    void Start()
    {


        // set wind particles speed to world speed
        var velocity = _windParticles.velocityOverLifetime;
        velocity.x = new ParticleSystem.MinMaxCurve(-_worldSpeed, -_worldSpeed);
        velocity.y = new ParticleSystem.MinMaxCurve(0, 0);//(0.1f * Random.Range(-10,10), 0.1f * Random.Range(-10, 10));
        velocity.z = new ParticleSystem.MinMaxCurve(0, 0);
        _levelColor = _worldcolors[0];
        _worldMaterial.SetColor("_Color", _levelColor);
        //_windMaterial.SetColor("_TintColor", _levelColor);
        StartCoroutine(ColorChanger());
    }
    #endregion

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
            while (Instance != null)
            {
                if (_player.activeSelf)
                {
                    hold = UnityEngine.Random.Range(_minColHoldTime, _maxColHoldTime);
                    transition = UnityEngine.Random.Range(_minColTransitionTime, _maxColTransitionTime);
                    while ((Time.time < colorTime + hold)&&_player.activeSelf&& Instance!=null) 
                    {
                        yield return null;
                    }
                    colorTime = Time.time;
                    oldColor = _levelColor;
                    Debug.Log("Change color");
                    while ((Time.time < colorTime + transition) && _player.activeSelf && Instance != null)
                    {
                        _levelColor = Color.Lerp(oldColor, _worldcolors[nextColor], (Time.time - colorTime) / transition);
                        // set material colours
                        _worldMaterial.SetColor("_Color", _levelColor);
                        // _windMaterial.SetColor("_TintColor", _levelColor);

                        yield return null; // should update ~60 times a sec
                    }
                    colorTime = Time.time;
                    nextColor = UnityEngine.Random.Range(0, _worldcolors.Count);
                }
                else
                {
                    colorTime = Time.time;
                    transition = 5;
                    oldColor = _levelColor;
                    Debug.Log("Change color");
                    while ((Time.time < colorTime + transition) && !Player.activeSelf && Instance != null) 
                    {
                        _levelColor = Color.Lerp(oldColor, _deadColor, (Time.time - colorTime) / transition);
                        //_worldMaterial.SetColor("_Color", _levelColor);
                        _worldMaterial.SetColor("_Color", _levelColor);
                        yield return null; // should update ~60 times a sec
                    }
                    colorTime = Time.time;
                    transition = 5;
                    oldColor = _levelColor;
                    Debug.Log("Change color");
                    while ((Time.time < colorTime + transition) && !Player.activeSelf && Instance != null)
                    {
                        _levelColor = Color.Lerp(oldColor, Color.clear, (Time.time - colorTime) / transition);
                        //_worldMaterial.SetColor("_Color", _levelColor);
                        _worldMaterial.SetColor("_Color", _levelColor);
                        if (_worldMaterial.GetColor("_Color")!=_levelColor)
                        {
                            Debug.Log("The Shader no work");
                        }
                        yield return null; // should update ~60 times a sec
                    }
                    yield break;
                }
                yield return null; // should update ~60 times a sec
            }
        }
    }
    private void Update()
    {
        if ( Player.activeSelf)
        {
            _timer += Time.deltaTime;
            foreach (TextMeshProUGUI tmp in _timeReadouts)
            {
                tmp.text = MathF.Floor(_timer) + ":" + MathF.Abs((int)((_timer % 1) * 100));
            }
        }
       
    }
    public void Restart() // reset everything back to how it started, not sure if I should just reload the scene?
    {
        // destroy objects tagged as "reset"
        GameObject[] all = GameObject.FindGameObjectsWithTag("Reset"); 
        Debug.Log("found " + all.Length + " gameobjects");
        foreach (GameObject g in all)
        {
            Destroy(g);
            Debug.Log("found and destroyed"+g.name);
        }
        // reset the player
        _player.SetActive(true);
        _player.transform.position = new Vector3(-4, -3, 0);
        // instantiate some new terrain (these should be pooled in future)
        GameObject go = Instantiate(_terrainList.pieces[0]);
        go.transform.position = Vector3.zero;
        int rng = UnityEngine.Random.Range(1, GameControl._instance._terrainList.pieces.Count);
        go = Instantiate(GameControl._instance._terrainList.pieces[rng]);
        go.transform.position = new Vector3(30,0,0);
        rng = UnityEngine.Random.Range(1, GameControl._instance._terrainList.pieces.Count);
        go = Instantiate(GameControl._instance._terrainList.pieces[rng]);
        go.transform.position = new Vector3(60, 0, 0);
        GameObject.FindAnyObjectByType<Trasher>().lastSpawned = go;
        // disable the gameover screen
        _gameOver.SetActive(false);
        _timer = 0;
        // reset our colours
        _levelColor = _worldcolors[0];
        _worldMaterial.SetColor("_Color", _levelColor);
        //_windMaterial.SetColor("_TintColor", _levelColor);
        StartCoroutine(ColorChanger());
    }

}
