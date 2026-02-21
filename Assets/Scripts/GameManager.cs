using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject enemyPrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);
    }

    public GameObject GetPlayer()
    {
        return playerPrefab;
    }

    public GameObject GetEnemy()
    {
        return enemyPrefab;
    }

    public void ExitBattle()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
