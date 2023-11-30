using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerColliderWithEnemyLevel2Modified : EnclosureCollision
{
    [SerializeField] private bool _spawnEnemies = false;
    [SerializeField] List<Transform> enemyLocations;
    [SerializeField] private bool EndCurrentObjective;
    [SerializeField] private ObjectiveManagerLevel2 objectiveManager;
    public static bool isActive = false;
    protected override void Start()
    {
        base.Start();
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isActive)
        {
            if (EndCurrentObjective && !_collidedBefore)
            {
                if (!ObjectiveManagerLevel2.activeObjective)
                {
                    objectiveManager.NextObjective();
                }
                else
                {
                    objectiveManager.currentObjective.OnComplete();
                }
            }
            
            if (_spawnEnemies && !_collidedBefore)
            {
                for (int i = 0; i < enemyLocations.Count; i++)
                {
                    if (enemyLocations[i] != null)
                    {
                        spawnManager.SpawnEnemy(enemyLocations[i], i);
                    }
                }

                SetCheckPoint();

            }
            _collidedBefore = true;
        }
    }
}
