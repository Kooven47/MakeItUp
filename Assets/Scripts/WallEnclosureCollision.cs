using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallEnclosureCollision : MonoBehaviour
{
    [SerializeField] private GameObject Enclosure;
    [SerializeField] private bool isHidden = true;
    [SerializeField] private bool isBossEntrance;
    [SerializeField] private bool spawnEnemies = false;
    [SerializeField] private bool triggerObjective = false;
    [SerializeField] private bool timedSpawn = false;
    [SerializeField] private float timeBetweenSpawns;
    [SerializeField] private float totalSpawnTime;
    
    [SerializeField] List<Transform> enemyLocations;
    public SpawnManager spawnManager;
    private bool _collidedBefore = false;

    // Start is called before the first frame update
    void Start()
    {
        if (isHidden)
            Enclosure.SetActive(false);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Enclosure.SetActive(true);

            if (triggerObjective)
            {
                ObjectiveManagerLevel3.OnUpdateObjective();
            }
            
            if (isBossEntrance && !_collidedBefore)
            {
                BossHealthBar.activateHealthBar?.Invoke();
            }
            
            if (spawnEnemies && !_collidedBefore)
            {
                for (int i = 0; i < enemyLocations.Count; i++)
                {
                    if (enemyLocations[i] != null)
                    {
                        if (timedSpawn)
                        {
                            spawnManager.CallTimedEnemySpawnCoroutine(enemyLocations[i], i, timeBetweenSpawns, totalSpawnTime);
                        }
                        else
                        {
                            spawnManager.SpawnEnemy(enemyLocations[i], i);
                        }
                    }
                }
            }

            _collidedBefore = true;
        }
    }
}
