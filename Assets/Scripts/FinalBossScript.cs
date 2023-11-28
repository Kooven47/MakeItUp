using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FinalBossScript : EnemyCore
{
    // Bosscore?
    protected EnemyStats _bossStats;
    protected EnemyAI _bossAI;
    protected Rigidbody2D _bossRB;
    protected SpriteRenderer _bossSpriteRender;
    protected GameObject _bossGameObject;
    [SerializeField] private GameObject _bossOrb;
    [SerializeField] private Sprite[] _explosioneffect = new Sprite[3];

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
    protected override void Start()
    {
        base.Start();
        EnemyStats.OnDeathWithType += KillUpdate;
        minionLocations = new List<Transform>();
        foreach (Transform child in GameObject.Find("GruLocations").GetComponent<Transform>())
        {
            minionLocations.Add(child);
        }
        spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        numMinions = minionLocations.Count;
        _bossGameObject = transform.parent.gameObject;
        _bossStats = _bossGameObject.GetComponent<EnemyStats>();
        _bossAI = _bossGameObject.GetComponent<EnemyAI>();
        _bossRB = _bossGameObject.GetComponent<Rigidbody2D>();
        
        _bossSpriteRender = GetComponent<SpriteRenderer>();
        _bossBubbleEmitter = _bossGameObject.transform.GetChild(2);
        _bossDustEmitter = _bossGameObject.transform.GetChild(3);
        SpawnMinions();
        // EnemyStats.OnDeath += KillUpdate;
    }

    // Update is called once per frame
    void Update()
    {
        // if (_bossStats.healthRatio <= 0.5f)
        // {
        //     _bossRB.gravityScale = 0;
        //     _bossAI.flyingEnabled = true;
        // }

        if (_canAttack)
        {
            SelectAttack();
            if (_attackIndex == 0)
            {
                ReadyAttack();
            }
            else if (_attackIndex == 1)
            {
                ChargingAttacks(5f,2f);
            }
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

    protected override void MeleeStrike()
    {
        List<Collider2D> targets = new List<Collider2D>();
        _hurtBox.gameObject.SetActive(true);
        Physics2D.OverlapCollider(_hurtBox,_hurtLayers,targets);
        _hurtBox.gameObject.SetActive(false);
        bool didHit = false;

        PlayerStats _playerStat;

        if (targets.Count != 0)
        {
            foreach(Collider2D col in targets)
            {
                if (col.CompareTag("Player"))
                {
                    Debug.Log("Hit the janitor!");
                    _playerStat = col.GetComponent<PlayerStats>();
                    if (!_playerStat.iFrame)
                    {
                        Vector2 direction = (col.transform.position - transform.position).normalized;
                        col.GetComponent<PlayerInterrupt>().Stagger(1,_knockBackVector * direction * 0.5f);
                        _playerStat.DamageCalc(_enemySkills[_attackIndex].damage,_bossStats.Attribute,false);
                        col.gameObject.GetComponent<DamageEffect>().TriggerEffect((int)_bossStats.Attribute);
                        didHit = true;
                    }
                    else
                    {
                        Debug.Log("Under Iframes");
                    }
                }
            }
        }

        if (didHit)
        {
            // Question mark acts as a null check to avoid invoking an action if not initialized somehow
            CameraFollow.StartShake?.Invoke();
        }
    }

    protected override void SelectAttack()
    {
        
        if (InDistance(_meleeRange))
        {
            _canAttack = false;
            _attackIndex = 0;
            _knockBackVector = EnumLib.KnockbackVector(_enemySkills[_attackIndex].force);
        }
        else if (InDistance(_projectileRange))
        {
            _canAttack = false;
            _attackIndex = 1;
            _knockBackVector = EnumLib.KnockbackVector(_enemySkills[_attackIndex].force);
        }
    }
}
