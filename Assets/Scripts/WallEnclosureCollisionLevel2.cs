using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallEnclosureCollisionLevel2 : EnclosureCollision
{
    [SerializeField] private GameObject Enclosure;
    [SerializeField] private bool isHidden = true;
    [SerializeField] private bool isBossEntrance;
    [SerializeField] private bool spawnEnemies = false;
    [SerializeField] private bool triggerObjective = false;
    [SerializeField] private bool timedSpawn = false;
    [SerializeField] private float timeBetweenSpawns;
    [SerializeField] private float totalSpawnTime;
    [SerializeField] private ObjectiveManagerLevel2 objectiveManagerLevel2;
    [SerializeField] List<Transform> enemyLocations;

    // Start is called before the first frame update
    protected override void Start()
    {
        Enclosure.SetActive(true);
        base.Start();
        if (isHidden)
            Enclosure.SetActive(false);
    }
    
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        // Enemy instead of player, don't lock out the freezer lol
        if (other.CompareTag("Enemy"))
        {
            print("lol we did it");
            Enclosure.SetActive(true);

            if (triggerObjective && !_collidedBefore)
            {
                if (!ObjectiveManagerLevel2.activeObjective)
                {
                    objectiveManagerLevel2.NextObjective();
                }
                else
                {
                    objectiveManagerLevel2.currentObjective.OnComplete();
                }
                // ObjectiveManagerLevel2.OnUpdateObjective();
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
