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
    protected EnemyCore _bossCore;
    protected SpriteRenderer _bossSpriteRender;
    protected GameObject _bossGameObject;
    protected Transform _bossBubbleEmitter;
    protected Transform _bossDustEmitter;
    private SpawnManager spawnManager;
    private List<Transform> minionLocations;
    private int numMinions, numAlive;
    private int numWet, numDry;
    [SerializeField] private Material DryOutline;
    [SerializeField] private Material WetOutline;
    [SerializeField] private Material defaultOutline;

    // Start is called before the first frame update
    void Start()
    {
        EnemyStats.OnDeathWithType += KillUpdate;
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
        _bossCore = GetComponent<EnemyCore>();
        _bossGameObject = this.gameObject;
        _bossSpriteRender = _bossGameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
        _bossBubbleEmitter = _bossGameObject.transform.GetChild(2);
        _bossDustEmitter = _bossGameObject.transform.GetChild(3);
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
            int enemyPrefabNumber = Random.Range(0, 6);
            Debug.Log($"Random number: {enemyPrefabNumber}");
            spawnManager.SpawnEnemy(minionLocations[i], enemyPrefabNumber);
            numAlive++;
            switch (enemyPrefabNumber)
            {
                case 0:
                    numWet++;
                    break;
                case 1:
                    numDry++;
                    break;
                case 2:
                    numWet++;
                    break;
                case 3:
                    numDry++;
                    break;
                case 4:
                    numWet++;
                    break;
                case 5:
                    numDry++;
                    break;
                default: break;
            }
        }
        
        Debug.Log("NumDry: " + numDry + "   NumWet: " + numWet);
        DecideBossAttribute();
    }

    private void KillUpdate(EnumLib.DamageType attribute)
    {
        numAlive--;
        if (attribute == EnumLib.DamageType.Dry) numDry--;
        if (attribute == EnumLib.DamageType.Wet) numWet--;
        Debug.Log("NumDry: " + numDry + "   NumWet: " + numWet);

        DecideBossAttribute();
    }

    private void OnDestroy()
    {
        EnemyStats.OnDeathWithType -= KillUpdate;
    }

    private void DecideBossAttribute()
    {
        if (numDry > numWet)
        {
            // Make attribute Dry
            _bossStats.Attribute = EnumLib.DamageType.Dry;
            _bossSpriteRender.material = DryOutline;
            _bossBubbleEmitter.gameObject.SetActive(false);
            _bossDustEmitter.gameObject.SetActive(true);
        }
        else if (numWet > numDry)
        {
            // Make attribute Wet
            _bossStats.Attribute = EnumLib.DamageType.Wet;
            _bossSpriteRender.material = WetOutline;
            _bossBubbleEmitter.gameObject.SetActive(true);
            _bossDustEmitter.gameObject.SetActive(false);
        }
        else if (numWet == numDry)
        {
            // Make attribute Neutral
            _bossStats.Attribute = EnumLib.DamageType.Neutral;
            _bossSpriteRender.material = defaultOutline;
            _bossBubbleEmitter.gameObject.SetActive(false);
            _bossDustEmitter.gameObject.SetActive(false);
        }
    }
}
