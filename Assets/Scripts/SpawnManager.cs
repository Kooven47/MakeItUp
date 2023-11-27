using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    public List<GameObject> enemyPrefabs; // Reference to your Enemy Prefab
    public Transform enemiesObject;
    private GameObject instantiatedEnemy;

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