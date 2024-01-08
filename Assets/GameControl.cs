using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class GameControl : MonoBehaviour
{
    public static GameControl Instance;
    [SerializeField] ParticleSystem windParticles;
    public GameObject gameOver;
    public GameObject player;
    [SerializeField] public TerrainList terrainList; 
    public float worldSpeed;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 120;
        Instance = this;

        var velocity = windParticles.velocityOverLifetime;
        velocity.x = new ParticleSystem.MinMaxCurve(-worldSpeed, -worldSpeed);
        velocity.y = new ParticleSystem.MinMaxCurve(0, 0);//(0.1f * Random.Range(-10,10), 0.1f * Random.Range(-10, 10));
        velocity.z = new ParticleSystem.MinMaxCurve(0, 0);
    }
    public void Restart()
    {
        GameObject[] all = GameObject.FindGameObjectsWithTag("Reset");
        Debug.Log("found " + all.Length + " gameobjects");
        foreach (GameObject g in all)
        {
            Destroy(g);
            Debug.Log("found and destroyed"+g.name);
        }
        player.SetActive(true);
        player.transform.position = new Vector3(-4, -3, 0);
        GameObject go = Instantiate(terrainList.pieces[0]);
        go.transform.position = Vector3.zero;
        int rng = Random.Range(1, GameControl.Instance.terrainList.pieces.Count);
        go = Instantiate(GameControl.Instance.terrainList.pieces[rng]);
        go.transform.position = new Vector3(30,0,0);
        rng = Random.Range(1, GameControl.Instance.terrainList.pieces.Count);
        go = Instantiate(GameControl.Instance.terrainList.pieces[rng]);
        go.transform.position = new Vector3(60, 0, 0);
        GameObject.FindAnyObjectByType<Trasher>().Lastspawned = go;
        gameOver.SetActive(false);
    }

}
