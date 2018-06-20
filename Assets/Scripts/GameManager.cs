using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    private static GameManager instance;

    public static GameManager Instance()
    {
        return instance;
    }

    public GameObject player;

    private Vector2 lastRespawnPoint;

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        lastRespawnPoint = Vector2.zero;

        SceneManager.sceneLoaded += UpdatePlayer;
    }

    /// <summary>
    /// Called after a scene is loaded
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    private void UpdatePlayer(Scene scene, LoadSceneMode mode)
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void RestartAtLastCheckPoint()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void UpdateRespawnPoint(Vector2 newRespwanPoint)
    {
        lastRespawnPoint = newRespwanPoint;
    }
}
