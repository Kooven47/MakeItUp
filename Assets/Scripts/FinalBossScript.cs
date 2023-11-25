using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FinalBossScript : MonoBehaviour
{
    // Bosscore?
    protected EnemyStats _bossStats;
    protected EnemyAI _bossAI;
    protected Rigidbody2D _bossRB;
    private SpawnManager spawnManager;
    private List<Transform> minionLocations;
    private int numMinions, numAlive;
    private int numWet, numDry;
    
    // Start is called before the first frame update
    void Start()
    {
        minionLocations = new List<Transform>();
        foreach (Transform child in GameObject.Find("GruLocations").GetComponent<Transform>())
        {
            minionLocations.Add(child);
        }
        spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        numMinions = minionLocations.Count;
        _bossStats = GetComponent<EnemyStats>();
        _bossAI = GetComponent<EnemyAI>();
        _bossRB = GetComponent<Rigidbody2D>();
        
        SpawnMinions();
        // EnemyStats.OnDeath += KillUpdate;
    }

    // Update is called once per frame
    void Update()
    {
        if (_bossStats.healthRatio <= 0.5f)
        {
            _bossRB.gravityScale = 0;
            _bossAI.flyingEnabled = true;
        }
    }

    private void SpawnMinions()
    {
        for (int i = 0; i < numMinions; i++)
        {
            int enemyPrefabNumber = Random.Range(0, 4);
            Debug.Log($"Random number: {enemyPrefabNumber}");
            spawnManager.SpawnEnemy(minionLocations[i], enemyPrefabNumber);
            numAlive++;
        }
    }

    // private void KillUpdate()
    // {
    //     numAlive--;
    //     if (EnemyStats.Attribute == EnumLib.DamageType.Dry) numDry--;
    //     if (EnemyStats.Attribute == EnumLib.DamageType.Wet) numWet--;
    // }
    //
    // private void OnDestroy()
    // {
    //     EnemyStats.OnDeath -= KillUpdate;
    // }
    
}
