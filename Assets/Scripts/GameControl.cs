using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

#region Enumerators
public enum TerrainTypes { Start, Easy, Medium, Hard }

#endregion

public class GameControl : MonoBehaviour
{
    #region Variables

    static GameControl _instance;
    float _timer = 0;
    
    List<TextMeshProUGUI> _timeReadouts = new List<TextMeshProUGUI>();
    [SerializeField] ParticleSystem _windParticles;
    [SerializeField] GameObject _gameOver;
    [SerializeField] GameObject _player;

    [SerializeField] float _worldSpeed;

    [SerializeField] Material _worldMaterial;
    Color _levelColor = Color.grey;
    [SerializeField] float _maxColTransitionTime;
    [SerializeField] float _minColTransitionTime;
    [SerializeField] float _maxColHoldTime;
    [SerializeField] float _minColHoldTime;
    [SerializeField] Color _deadColor;
    [SerializeField] List<Color> _worldcolors = new List<Color>();

    [SerializeField] Vector3 _firstTerrainPos = Vector3.zero;
    [SerializeField] List<TerrainList> _terrainLists;
    List<List<GameObject>> _terrainPools = new List<List<GameObject>>();
    List<GameObject> _terrainBlank = new List<GameObject>();

    // Terrain vars previously in Trasher.cs
    private GameObject _lastSpawned;
    private float _lastSpawnSize;


    #endregion

    #region Properties

    public static GameControl Instance { get => _instance ?? null; }
    public GameObject GameOver { get => _gameOver ?? null; }
    public GameObject Player { get => _player ?? null; }
    public float WorldSpeed { get => _worldSpeed; }
    public static float RunTimer { get => _instance._timer; }
    public static Color LevelColor { get => _instance._levelColor; }

    public static List<List<GameObject>> Pools { get => _instance._terrainPools; }

    public static GameObject LastSpawned { get => _instance._lastSpawned; }
    public static float LastSpawnedSize { get => _instance._lastSpawnSize; set => _instance._lastSpawnSize = value; }
    public static Vector3 FirstTerrainPos { get => _instance?._firstTerrainPos ?? Vector3.zero; }

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
        SetupTerrain();
    }

    void Start()
    {
        //spawn the level
        SpawnTerrain(_terrainPools[(int)TerrainTypes.Start]);
        SpawnTerrain(_terrainPools[(int)TerrainTypes.Easy]);
        SpawnTerrain(_terrainPools[(int)TerrainTypes.Easy]);


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

    void SetupTerrain() // load then disable all terrain pieces and add to lists of available terrain by type (start(0) easy(1) med(2) hard(3))
    {
        int i = 0;
        while (i < _terrainLists.Count)
        {
            List<GameObject> newTerrainList = new List<GameObject>();
            // for all lists other than start we want multiples of each terrain piece, so loop through twice
            for (int j = 0; (j < 2 && i > 0)||j<1; j++)
            {
                foreach (GameObject terrainPiece in _terrainLists[i].pieces)
                {
                    GameObject newTerrain = Instantiate(terrainPiece);
                    newTerrainList.Add(newTerrain);
                    newTerrain.SetActive(false);
                    newTerrain.GetComponent<TerrainMovement>()?.SetPool(newTerrainList);
                }
            }
            _terrainPools.Add(newTerrainList);
            i++;
        }
    }

    public void SpawnTerrain()
    {
        GameObject toSpawn;
        do
        {
            int rng = UnityEngine.Random.Range(0, _terrainPools.Count);
            toSpawn = _terrainPools[1][rng];
            if (toSpawn == null)
                Debug.Log("Terrain Pool missing piece at index: " + rng);
        } while (toSpawn == null);
        toSpawn.SetActive(true);
        _lastSpawned = toSpawn;
    }

    public void SpawnTerrain(List<GameObject> pool)
    {
        GameObject toSpawn;
        do
        {
            int rng = UnityEngine.Random.Range(0, pool.Count);
            Debug.Log("rng = " + rng);
            toSpawn = pool[rng];
            if (toSpawn == null)
                Debug.Log("Terrain Pool missing piece at index: " + rng);
        } while (toSpawn == null);
        toSpawn.SetActive(true);
        _lastSpawned = toSpawn;
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
            while (Instance != null)
            {
                if (_player.activeSelf)
                {
                    hold = UnityEngine.Random.Range(_minColHoldTime, _maxColHoldTime);
                    transition = UnityEngine.Random.Range(_minColTransitionTime, _maxColTransitionTime);
                    while ((Time.time < colorTime + hold)&&_player.activeSelf&& Instance!=null) 
                    {
                        yield return new WaitForFixedUpdate();
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

                        yield return new WaitForFixedUpdate(); 
                    }
                    colorTime = Time.time;
                    nextColor = UnityEngine.Random.Range(0, _worldcolors.Count);
                }
                else
                {
                    colorTime = Time.time;
                    transition = 3;
                    oldColor = _levelColor;
                    Debug.Log("Change color");
                    while ((Time.time < colorTime + transition) && !Player.activeSelf && Instance != null) 
                    {
                        _levelColor = Color.Lerp(oldColor, _deadColor, (Time.time - colorTime) / transition);
                        _worldMaterial.SetColor("_Color", _levelColor);
                        yield return new WaitForFixedUpdate();
                    }
                    colorTime = Time.time;
                    transition = 3;
                    oldColor = _levelColor;
                    Debug.Log("Change color");
                    // commented so I can still see level when dead for testing
                    while ((Time.time < colorTime + transition) && !Player.activeSelf && Instance != null)
                    {
                        _levelColor = Color.Lerp(oldColor, Color.clear, (Time.time - colorTime) / transition);
                        _worldMaterial.SetColor("_Color", _levelColor);
                        if (_worldMaterial.GetColor("_Color") != _levelColor)
                        {
                            Debug.Log("The Shader no work");
                        }
                        yield return new WaitForFixedUpdate();
                    }
                    yield break;
                }
                yield return null; 
            }
        }
    }
    private void Update()
    {
        if ( Player.activeSelf) // only run timer when player is alive
        {
            _timer += Time.deltaTime;
            foreach (TextMeshProUGUI tmp in _timeReadouts)
            {
                tmp.text = MathF.Floor(_timer) + ":" + MathF.Abs((int)((_timer % 1) * 100));
            }
        }
       
    }
    public void Restart() // reset everything back to how it started
    {
        SceneManager.LoadScene(0);

    }

}
