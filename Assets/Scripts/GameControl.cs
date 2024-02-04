using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class GameControl : MonoBehaviour
{
    #region Variables

    static GameControl _instance;
    [SerializeField] ParticleSystem _windParticles;
    [SerializeField] GameObject _gameOver;
    [SerializeField] GameObject _player;
    [SerializeField] public TerrainList _terrainList; 
    [SerializeField] float _worldSpeed;
    [SerializeField] List<Color> _worldcolors = new List<Color>();

    #endregion
    #region Properties

    public static GameControl Instance { get => _instance ?? null; }
    public GameObject GameOver { get => _gameOver ?? null; }
    public GameObject Player { get => _player ?? null; }
    public float WorldSpeed { get => _worldSpeed; }

    #endregion

    // Start is called before the first frame update
    private void Awake()
    {
        // setup platform specific elements
#if UNITY_WEBGL
        Debug.Log("Running WebGL");
        foreach (GameObject ui in GameObject.FindGameObjectsWithTag("WebUI"))
        {
            Debug.Log("found UI object " + ui.name);
            if (ui.name.Contains("GameOver"))
            {
                _gameOver = ui;
            }
            ui.SetActive(false);
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
    }
    void Start()
    {
        Application.targetFrameRate = 120;
        _instance = this;

        // set wind particles speed to world speed
        var velocity = _windParticles.velocityOverLifetime;
        velocity.x = new ParticleSystem.MinMaxCurve(-_worldSpeed, -_worldSpeed);
        velocity.y = new ParticleSystem.MinMaxCurve(0, 0);//(0.1f * Random.Range(-10,10), 0.1f * Random.Range(-10, 10));
        velocity.z = new ParticleSystem.MinMaxCurve(0, 0);
    }
    public void Restart() // reset everything back to how it started
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
        int rng = Random.Range(1, GameControl._instance._terrainList.pieces.Count);
        go = Instantiate(GameControl._instance._terrainList.pieces[rng]);
        go.transform.position = new Vector3(30,0,0);
        rng = Random.Range(1, GameControl._instance._terrainList.pieces.Count);
        go = Instantiate(GameControl._instance._terrainList.pieces[rng]);
        go.transform.position = new Vector3(60, 0, 0);
        GameObject.FindAnyObjectByType<Trasher>().lastSpawned = go;
        // disable the gameover screen
        _gameOver.SetActive(false);
    }

}
