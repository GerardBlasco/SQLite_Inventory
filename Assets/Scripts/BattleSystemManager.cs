using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum BattleSatte { START, PLAYER_TURN, ENEMY_TURN, WON, LOST }

public class BattleSystemManager : MonoBehaviour
{
    public static BattleSystemManager instance;

    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject enemyPrefab;

    [SerializeField] List<GameObject> enemySpawnPositions = new List<GameObject>();

    private int enemiesToSpawn;
    private bool started = false;

    public BattleSatte state;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }

        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "BattleScene" && !started)
        {
            started = true;
            SetupBattle();
        }
    }

    public void StartBattleScene(int enemiesToSpawn)
    {
        this.enemiesToSpawn = enemiesToSpawn;
        SceneManager.LoadScene("BattleScene");
    }

    void SetupBattle()
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Spawn"))
        {
            enemySpawnPositions.Add(go);
        }

        state = BattleSatte.START;
        InputManager.instance.DisableInput();

        playerPrefab = GameManager.instance.GetPlayer();
        enemyPrefab = GameManager.instance.GetEnemy();

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Instantiate(enemyPrefab, enemySpawnPositions[i].transform.position, Quaternion.Euler(0f, -90f, 0f));
        }
    }
}
