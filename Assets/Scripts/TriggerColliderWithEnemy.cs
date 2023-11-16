using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerColliderWithEnemy : EnclosureCollision
{
    [SerializeField] private bool _spawnEnemies = false;
    [SerializeField] List<Transform> enemyLocations;
    protected override void Start()
    {
        base.Start();
    }
    private bool _collidedBefore = false;
    [SerializeField] private bool EndCurrentObjective;
    [SerializeField] private bool _spawnEnemies = false;
    [SerializeField] List<Transform> enemyLocations;
    public SpawnManager spawnManager;
    [SerializeField] private ObjectiveManagerLevel3 objectiveManager;

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (EndCurrentObjective)
            {
                objectiveManager.currentObjective.OnComplete();
            }
            // ObjectiveManagerLevel3.OnUpdateObjective();
            if (_spawnEnemies && !_collidedBefore)
            {
                for (int i = 0; i < enemyLocations.Count; i++)
                {
                    if (enemyLocations[i] != null)
                    {
                        spawnManager.SpawnEnemy(enemyLocations[i], i);
                    }
                }

                _collidedBefore = true;
            }
        }
    }
}
