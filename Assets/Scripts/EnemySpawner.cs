using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemies;
    public float patrolDistanceX = 1f;
    public float patrolDistanceY = 1f;
    public float sqrDistanceRespawn = 3f;
    GameObject goEnmey;
    PlayerMove player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerMove>();
        SpawnEnemy();
    }

    private void Update()
    {
        float sqrDistanceToPlayer = (transform.position - player.transform.position).sqrMagnitude;
        if (goEnmey == null && sqrDistanceToPlayer > sqrDistanceRespawn)
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        int index = Random.Range(0, enemies.Length);
        goEnmey = Instantiate(enemies[index], transform.position, transform.rotation);
        EnemyBehavior enemy = goEnmey.GetComponent<EnemyBehavior>();
        enemy.SetPatrolPosition(patrolDistanceX, patrolDistanceY);
    }
}
