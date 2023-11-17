using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallEnclosureCollision : EnclosureCollision
{
    [SerializeField] private GameObject Enclosure;
    [SerializeField] private bool isHidden = true;
    [SerializeField] private bool isBossEntrance;
    [SerializeField] private bool spawnEnemies = false;
    [SerializeField] private bool triggerObjective = false;
    [SerializeField] private bool timedSpawn = false;
    [SerializeField] private float timeBetweenSpawns;
    [SerializeField] private float totalSpawnTime;
    [SerializeField] private ObjectiveManagerLevel3 objectiveManagerLevel3;
    [SerializeField] List<Transform> enemyLocations;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        if (isHidden)
            Enclosure.SetActive(false);
    }
    
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            print("lol we did it");
            Enclosure.SetActive(true);

            if (triggerObjective && !_collidedBefore)
            {
                if (!ObjectiveManagerLevel3.activeObjective)
                {
                    objectiveManagerLevel3.NextObjective();
                }
                else
                {
                    objectiveManagerLevel3.currentObjective.OnComplete();
                }
                ObjectiveManagerLevel3.OnUpdateObjective();
                //SetCheckPoint();
            }
            
            if (isBossEntrance && !_collidedBefore)
            {
                BossHealthBar.activateHealthBar?.Invoke();
                SetCheckPoint();
            }
            
            if (spawnEnemies && !_collidedBefore)
            {
                //SetCheckPoint();
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
            // gameObject.SetActive(false);
        }
    }
}
