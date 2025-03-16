using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : BaseObjectPooling
{
    public static EnemySpawner instance;

    [SerializeField]
    List<GameObject> enemyPrefabs;

    [SerializeField]
    List<Transform> spawnPosition;

    [SerializeField] Transform enemyParent;
    [SerializeField] int enemyPoolSize;

    private float spawnInterval = 10f; // Start at 10 seconds
    private const float minSpawnInterval = 5f;
    private const float reductionAmount = 1f;
    [SerializeField] float spawnRadius = 1f;

    [SerializeField] int maxSpawn = 10;
    [SerializeField] int totalSpawn = 0;
    bool canSpawn = true;

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnInitializationPoolObject(enemyPoolSize, pooledObjectList, enemyPrefabs, enemyParent);
        StartCoroutine(SpawnEnemies());
        SpawnEnemy(true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            if (canSpawn && GameplayManager.instance.gameState == GameState.exploring)
            {

                yield return new WaitForSeconds(spawnInterval);

                SpawnEnemy();
                // Decrease interval but never go below 5 seconds
                if (spawnInterval > minSpawnInterval)
                {
                    spawnInterval -= reductionAmount;
                }
            }
            else
            {
                yield return new WaitForSeconds(spawnInterval);
            }

        }
    }
    void SpawnEnemy(bool atStart = false)
    {
        if (canSpawn)
        {
            GameObject enemy = GetPooledObject();
            if (enemy != null)
            {
                Transform spawnPoint;
                if (atStart)
                {
                    spawnPoint = spawnPosition[0];
                }
                else
                    spawnPoint = spawnPosition[Random.Range(0, spawnPosition.Count)];

                Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
                Vector3 spawnPos = spawnPoint.position + new Vector3(randomOffset.x, randomOffset.y, 0); // Keep Z = 0 for 2D

                enemy.transform.position = spawnPoint.position;
                enemy.SetActive(true);
                EnemyController enemyController = enemy.GetComponent<EnemyController>();
                enemyController.InitializeParty();

                if (GameplayManager.instance.gameState == GameState.battle)
                {
                    enemyController.CanMove = false;
                }
                else
                {
                    enemyController.CanMove = true;
                }

                GameplayManager.instance.AddEnemy(enemyController);
                totalSpawn++;
                CheckTotalSpawn();
            }
        }
    }
    public void DestroyEnemy()
    {
        Debug.Log("enemy lose");
        if (totalSpawn != 0)
        {
            totalSpawn--;
        }
        CheckTotalSpawn();
    }
    void CheckTotalSpawn()
    {
        if (totalSpawn < maxSpawn)
        {
            canSpawn = true;
        }
        else
        {
            canSpawn = false;
        }
    }
}
