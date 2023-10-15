using Unity.VisualScripting;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject enemyPrefab; // Reference to your Enemy Prefab
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

    public void SpawnEnemy(Transform spawnPointObject)
    {
        // Instantiate an enemy at the spawn point's position and rotation
        foreach (Transform spawnPoint in spawnPointObject) 
        {
            instantiatedEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            instantiatedEnemy.transform.SetParent(enemiesObject);

            PlayerControllerJanitor.enemyAIList = FindObjectsOfType<EnemyAI>();
        }
    }
}