using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    public List<GameObject> enemyPrefabs; // Reference to your Enemy Prefab
    public Transform enemiesObject;
    //public Transform spawnPoint;   // Reference to your Spawn Point
    //public float spawnInterval = 2f; // Time between enemy spawns

    //private float nextSpawnTime = 0f;
    private GameObject instantiatedEnemy;

    void Update()
    {
        // Check if it's time to spawn a new enemy
        //if (Time.time >= nextSpawnTime)
        //{
        //    SpawnEnemy();
        //    nextSpawnTime = Time.time + spawnInterval;
        //}
    }

    public void SpawnEnemy(Transform spawnPointObject, int prefabNumber)
    {
        // Instantiate an enemy at the spawn point's position and rotation
        foreach (Transform spawnPoint in spawnPointObject) 
        {
            instantiatedEnemy = Instantiate(enemyPrefabs[prefabNumber], spawnPoint.position, spawnPoint.rotation);
            instantiatedEnemy.transform.SetParent(enemiesObject);

            PlayerControllerJanitor.enemyAIList = FindObjectsOfType<EnemyAI>();
        }
    }

    public void CallTimedEnemySpawnCoroutine(Transform spawnPointObject, int prefabNumber, float timeBetweenSpawns, float totalSpawnTime)
    {
        StartCoroutine(TimedEnemySpawn(spawnPointObject, prefabNumber, timeBetweenSpawns, totalSpawnTime));
    }

    public IEnumerator TimedEnemySpawn(Transform spawnPointObject, int prefabNumber, float timeBetweenSpawns, float totalSpawnTime)
    {
        float curTime = 0f;

        while (curTime <= totalSpawnTime)
        {
            SpawnEnemy(spawnPointObject, prefabNumber);

            yield return new WaitForSeconds(timeBetweenSpawns);
            curTime += timeBetweenSpawns;
        }
    }
}