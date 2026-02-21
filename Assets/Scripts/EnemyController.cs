using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private int id;
    [SerializeField] int enemyBackup;

    private void Start()
    {
        enemyBackup = Random.Range(1, 4);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            BattleSystemManager.instance.StartBattleScene(enemyBackup);
        }
    }
}
