using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerColliderWithEnemy : MonoBehaviour
{
    private bool _collidedBefore = false;
    [SerializeField] private bool _spawnEnemies = false;
    [SerializeField] List<Transform> enemyLocations;
    public SpawnManager spawnManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ObjectiveManagerLevel3.OnUpdateObjective();
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
