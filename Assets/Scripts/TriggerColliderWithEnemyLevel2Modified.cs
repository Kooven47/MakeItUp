using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerColliderWithEnemyLevel2Modified : EnclosureCollision
{
    [SerializeField] private bool _spawnEnemies = false;
    [SerializeField] List<Transform> enemyLocations;
    [SerializeField] private bool EndCurrentObjective;
    [SerializeField] private ObjectiveManagerLevel2 objectiveManager;
    [SerializeField] private GameObject _beans;
    [SerializeField] private Animator _animator;
    public static bool isActive;
    protected override void Start()
    {
        isActive = false;
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
                        _animator.SetBool("RunAnim", true);
                        StartCoroutine(WaitSeconds());
                        _beans.SetActive(false);
                        spawnManager.SpawnEnemy(enemyLocations[i], i);
                    }
                }

                SetCheckPoint();
                // Debug.Log("RunAnim");
            }
            _collidedBefore = true;
        }
    }
    IEnumerator WaitSeconds()
    {
        yield return new WaitForSeconds(2.7f);
        GameObject Freezer = GameObject.Find("Freezer Variant(Clone)");
        EnemyAI enemyAIComponent = Freezer.GetComponent<EnemyAI>();
        enemyAIComponent.pathfind = true;
    }
}
