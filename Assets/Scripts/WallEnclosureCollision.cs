using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallEnclosureCollision : MonoBehaviour
{
    [SerializeField] private GameObject Enclosure;
    [SerializeField] private bool _isHidden = true;
    [SerializeField] private bool _isBossEntrance;
    [SerializeField] private bool _spawnEnemies = false;
    [SerializeField] List<Transform> enemyLocations;
    public SpawnManager spawnManager;
    private bool _collidedBefore = false;

    // Start is called before the first frame update
    void Start()
    {
        if (_isHidden)
            Enclosure.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Enclosure.SetActive(true);
            if (_isBossEntrance && !_collidedBefore)
            {
                BossHealthBar.activateHealthBar?.Invoke();
            }
            if (_spawnEnemies && !_collidedBefore)
            {
                if (enemyLocations[0] != null)
                    spawnManager.SpawnEnemy(enemyLocations[0], 0);
                if (enemyLocations[1] != null)
                    spawnManager.SpawnEnemy(enemyLocations[1], 1);
            }

            _collidedBefore = true;
        }
    }
}
