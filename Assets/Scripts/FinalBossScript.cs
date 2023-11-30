using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FinalBossScript : EnemyCore
{
    // Bosscore?
    protected EnemyAI _bossAI;
    protected Rigidbody2D _bossRB;
    protected SpriteRenderer _bossSpriteRender;
    protected GameObject _bossGameObject;
    [SerializeField] private GameObject _bossOrb;
    [SerializeField] private GameObject[] _explosioneffect = new GameObject[3];

    protected Transform _bossBubbleEmitter;
    protected Transform _bossDustEmitter;
    private SpawnManager spawnManager;
    private List<Transform> minionLocations;
    private int numMinions, numAlive;
    private int numWet, numDry;
    [SerializeField] private Material DryOutline;
    [SerializeField] private Material WetOutline;
    [SerializeField] private Material defaultOutline;

    private Coroutine _orbCooldown = null;

    private bool _shockWaveActive = false;

    public enum ShockWaveIndex{Dirt1,Dirt2,Wave1,Wave2,Neutral};

    private EnumLib.DamageType _lastRecorded = EnumLib.DamageType.Neutral;

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
        _enemyStats = _bossGameObject.GetComponent<EnemyStats>();
        _bossAI = _bossGameObject.GetComponent<EnemyAI>();
        _bossRB = _bossGameObject.GetComponent<Rigidbody2D>();
        
        _bossSpriteRender = GetComponent<SpriteRenderer>();
        _bossBubbleEmitter = _bossGameObject.transform.GetChild(2);
        _bossDustEmitter = _bossGameObject.transform.GetChild(3);
        SpawnMinions();

        foreach(GameObject g in _explosioneffect)
        {
            g.SetActive(false);
        }
        // EnemyStats.OnDeath += KillUpdate;
    }

    protected override void Recovery()
    {
        _anim.SetTrigger("endAttack");
        base.Recovery();
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
            _enemyStats.Attribute = EnumLib.DamageType.Dry;
            _bossSpriteRender.material = DryOutline;
            _bossBubbleEmitter.gameObject.SetActive(false);
            _bossDustEmitter.gameObject.SetActive(true);
        }
        else if (numWet > numDry)
        {
            // Make attribute Wet
            _enemyStats.Attribute = EnumLib.DamageType.Wet;
            _bossSpriteRender.material = WetOutline;
            _bossBubbleEmitter.gameObject.SetActive(true);
            _bossDustEmitter.gameObject.SetActive(false);
        }
        else if (numWet == numDry)
        {
            // Make attribute Neutral
            _enemyStats.Attribute = EnumLib.DamageType.Neutral;
            _bossSpriteRender.material = defaultOutline;
            _bossBubbleEmitter.gameObject.SetActive(false);
            _bossDustEmitter.gameObject.SetActive(false);
        }
    }

    private IEnumerator OrbCooldown(float timer)
    {
        yield return new WaitForSeconds(timer);
        _orbCooldown = null;
    }

    protected override IEnumerator ChargeTimers(Vector2 chargeTime, bool isWindUp)
    {
        
        if (isWindUp)
        {
            float charging = chargeTime.x * 0.9f;
            float finalCharge = chargeTime.x - charging;

            if (numAlive == 0)
            {
                _lastRecorded = (EnumLib.DamageType)Random.Range(0,3);
            }
            else
                _lastRecorded = _enemyStats.Attribute;
            
            _bossOrb.SetActive(true);
            SetOrb((int)_lastRecorded);
            
            while (charging > 0f)
            {
                if (_lastRecorded != _enemyStats.Attribute)
                {
                    _lastRecorded = _enemyStats.Attribute;
                    SetOrb((int)_lastRecorded);
                }
                charging -= Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }

            yield return new WaitForSeconds(finalCharge);
            _bossOrb.SetActive(false);
            _anim.SetTrigger("release");
            _attackWindUp = StartCoroutine(ChargeTimers(chargeTime,false));
        }
        else
        {
            yield return new WaitForSeconds(chargeTime.y);
            _anim.SetTrigger("endAttack");
            _attackWindUp = null;
            _orbCooldown = StartCoroutine(OrbCooldown((numAlive > 0 ? 12f : 8f)));
            SummonShockwave(false);
            Recovery();
        }
    }

    private bool DealDamage(Collider2D col,bool didHit)
    {
        PlayerStats _playerStat = col.GetComponent<PlayerStats>();
        if (!_playerStat.iFrame)
        {
            Vector2 direction = (col.transform.position - transform.position).normalized;
            col.GetComponent<PlayerInterrupt>().Stagger(1,_knockBackVector * direction * 0.5f);
            _playerStat.DamageCalc(_enemySkills[_attackIndex].damage,_lastRecorded,false);
            col.gameObject.GetComponent<DamageEffect>().TriggerEffect((int)_lastRecorded);
            didHit = true;
        }
        else
        {
            Debug.Log("Under Iframes as the boss!");
        }

        return didHit;
    }

    private void ShockWaveScan()
    {
        Collider2D explosion1 = null,explosion2 = null;
        bool didHit = false, marked = false;
        List<Collider2D> targets = new List<Collider2D>();

        switch(_lastRecorded)
        {
            case EnumLib.DamageType.Neutral:
                explosion1 = _explosioneffect[(int)ShockWaveIndex.Neutral].GetComponent<Collider2D>();
            break;
            case EnumLib.DamageType.Dry:
                explosion1 = _explosioneffect[(int)ShockWaveIndex.Dirt1].GetComponent<Collider2D>();
                explosion2 = _explosioneffect[(int)ShockWaveIndex.Dirt2].GetComponent<Collider2D>();
            break;

            case EnumLib.DamageType.Wet:
                explosion1 = _explosioneffect[(int)ShockWaveIndex.Wave1].GetComponent<Collider2D>();
                explosion2 = _explosioneffect[(int)ShockWaveIndex.Wave2].GetComponent<Collider2D>();
            break;
        }
        
        if (explosion1 != null)
        {
            Physics2D.OverlapCollider(explosion1,_hurtLayers,targets);

            if (targets.Count != 0)
            {
                foreach(Collider2D col in targets)
                {
                    if (col.CompareTag("Player"))
                    {
                        Debug.Log("Hit the janitor with shockwave");
                        didHit = DealDamage(col,didHit);
                        marked = true;
                    }
                }
            }
            else
            {
                Debug.Log("No targets scanned for boss");
            }
        }
        
        if (explosion2 != null && !marked)
        {
            Physics2D.OverlapCollider(explosion2,_hurtLayers,targets);

            if (targets.Count != 0)
            {
                foreach(Collider2D col in targets)
                {
                    if (col.CompareTag("Player"))
                    {
                        Debug.Log("Hit the janitor with shockwave");
                        didHit = DealDamage(col,didHit);
                        marked = true;
                    }
                    else
                    {
                        Debug.Log("Super punched "+col.name+" on Layer: "+col.gameObject.layer);
                    }
                }
            }
            else
            {
                Debug.Log("No targets scanned for boss");
            }
        }


        if (didHit)
        {
            // Question mark acts as a null check to avoid invoking an action if not initialized somehow
            CameraFollow.StartShake?.Invoke();
        }
    }

    private void SetOrb(int i)
    {
        _bossOrb.GetComponent<Animator>().SetInteger("Element",i);
    }

    private void SummonShockwave()
    {
        SummonShockwave(true);
    }

    private void SummonShockwave(bool setActive)
    {
        GameObject explosion1,explosion2;
        switch(_lastRecorded)
        {
            case EnumLib.DamageType.Neutral:
                explosion1 = _explosioneffect[(int)ShockWaveIndex.Neutral];
                explosion1.SetActive(setActive);
            break;
            case EnumLib.DamageType.Dry:
                explosion1 = _explosioneffect[(int)ShockWaveIndex.Dirt1];
                explosion2 = _explosioneffect[(int)ShockWaveIndex.Dirt2];
                explosion1.SetActive(setActive);
                explosion2.SetActive(setActive);
            break;
            case EnumLib.DamageType.Wet:
                explosion1 = _explosioneffect[(int)ShockWaveIndex.Wave1];
                explosion2 = _explosioneffect[(int)ShockWaveIndex.Wave2];

                explosion1.transform.localPosition = Vector2.zero;
                explosion2.transform.localPosition = Vector2.zero;
                
                explosion1.SetActive(setActive);
                explosion2.SetActive(setActive);

                if (setActive)
                {
                    explosion1.GetComponent<Rigidbody2D>().AddForce(new Vector2(-300f,0f));
                    explosion2.GetComponent<Rigidbody2D>().AddForce(new Vector2(300f,0f));
                }
                else
                {
                    explosion1.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    explosion2.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                }
            break;
        }
        _shockWaveActive = setActive;
    }

    protected override void MeleeStrike()
    {
        Debug.Log("Calling melee strike at "+_hurtBox.transform.localPosition);
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
                    _playerStat = col.GetComponent<PlayerStats>();
                    if (!_playerStat.iFrame)
                    {
                        Vector2 direction = (col.transform.position - transform.position).normalized;
                        col.GetComponent<PlayerInterrupt>().Stagger(1,_knockBackVector * direction * 0.5f);
                        _playerStat.DamageCalc(_enemySkills[_attackIndex].damage,_enemyStats.Attribute,false);
                        col.gameObject.GetComponent<DamageEffect>().TriggerEffect((int)_enemyStats.Attribute);
                        didHit = true;
                    }
                    else
                    {
                        Debug.Log("Under Iframes as the boss!");
                    }
                }
                else
                {
                    Debug.Log("Super punched "+col.name+" on Layer: "+col.gameObject.layer);
                }
            }
        }
        else
        {
            Debug.Log("No targets scanned for boss");
        }

        if (didHit)
        {
            // Question mark acts as a null check to avoid invoking an action if not initialized somehow
            CameraFollow.StartShake?.Invoke();
        }
    }

    protected override void SelectAttack()
    {
        
        
        if (InDistance(_meleeRange) && _orbCooldown == null)
        {
            _canAttack = false;
            _attackIndex = 1;
            _knockBackVector = EnumLib.KnockbackVector(_enemySkills[_attackIndex].force);
        }
        else if (InDistance(_meleeRange))
        {
            _canAttack = false;
            _attackIndex = 0;
            _knockBackVector = EnumLib.KnockbackVector(_enemySkills[_attackIndex].force);
        }
    }

    void Update()
    {
        // if (_enemyStats.healthRatio <= 0.5f)
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

        if (_shockWaveActive)
        {
            ShockWaveScan();
        }
    }
}
