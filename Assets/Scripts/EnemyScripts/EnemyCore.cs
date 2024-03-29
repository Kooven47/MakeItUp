using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyCore : MonoBehaviour
{
    [SerializeField] protected bool _isMelee = true;
    [SerializeField] protected Vector2 _projectileRange = new Vector2(3f,2f);
    [SerializeField] protected Vector2 _meleeRange = new Vector2(3f,2f);
    [SerializeField] protected ContactFilter2D _hurtLayers;
    [SerializeField] protected Collider2D _hurtBox;
    [SerializeField] protected EnemyAbility[] _enemySkills = new EnemyAbility[2];
    [SerializeField] protected AnimatorOverrideController _animOverride;
    [SerializeField] protected float _angleOffset = -90f;
    protected const string ATTACK ="Attack", RECOVERY = "Recovery",ATTACKCHARGE = "AttackCharge", ATTACKRELEASE ="AttackRelease";
    protected Coroutine _idleTimer, _attackWindUp;

    protected EnemyStats _enemyStats;

    protected Animator _anim;

    protected bool _canAttack = false;

    protected bool _inAttack = false;// Used during charge attacks

    protected int _attackIndex = -1;

    protected Transform _target;

    public Action<bool> StartArmor;

    protected Vector2 _knockBackVector;

    protected virtual void Start()
    {
        _enemyStats = transform.parent.GetComponent<EnemyStats>();
        _idleTimer = StartCoroutine(IdleTimer(3f));
        _animOverride = Instantiate(_animOverride);
        _anim = GetComponent<Animator>();
        _anim.runtimeAnimatorController = _animOverride;
        _target = GameObject.Find("Janitor").transform;
        if (_hurtBox == null)
            _hurtBox = transform.GetChild(0).GetComponent<Collider2D>();
    }

    public Quaternion AnglefromVector(Vector3 dir)
    {
        // dir = dir.normalized;
        float n = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg - _angleOffset;
        Quaternion q = Quaternion.AngleAxis(n,Vector3.forward);
            

        Debug.Log("Measured degree is "+n);
        
        return q;
    }

    protected bool InDistance(Vector2 range)
    {
        float x_dist = Mathf.Abs(transform.position.x - _target.position.x);
        float y_dist = Mathf.Abs(transform.position.y - _target.position.y);

        return x_dist <= range.x && y_dist <= range.y;
    }

    protected virtual void ProjectileFire()
    {
        int skillIndex = _attackIndex;
        if (skillIndex == -1)
            skillIndex = 0;

        ProjectileManager.createProjectile?.Invoke(transform.position,Direction(),_enemySkills[skillIndex],_enemyStats.attack);
    }

    protected virtual void MeleeStrike()
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
                        _playerStat.DamageCalc(_enemySkills[_attackIndex].damage + _enemyStats.attack,_enemySkills[_attackIndex].attribute,false);
                        col.gameObject.GetComponent<DamageEffect>().TriggerEffect((int)_enemySkills[_attackIndex].attribute);
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

    protected virtual void Recovery()
    {
        _canAttack = false;
        StartArmor?.Invoke(false);

        if (_attackWindUp != null)
        {
            StopCoroutine(_attackWindUp);
            _attackWindUp = null;
        }
        
        if (_idleTimer != null || _attackIndex < 0)
        {
            if (_idleTimer != null)
                StopCoroutine(_idleTimer);
            
            _idleTimer = StartCoroutine(IdleTimer(3f));
        }
        else
        {
            _idleTimer = StartCoroutine(IdleTimer(_enemySkills[_attackIndex].idleTime));
        }
        _attackIndex = -1;
    }

    public Vector3 Direction()
    {
        return (_target.position - transform.position).normalized;
    }

    protected virtual void SelectAttack()
    {
        // if (InDistance(_projectileRange))
        // {
        //     _canAttack = false;
        //     _attackIndex = 1;
        // }
        if (InDistance(_meleeRange) && _isMelee)
        {
            _canAttack = false;
            _attackIndex = 0;
            _knockBackVector = EnumLib.KnockbackVector(_enemySkills[_attackIndex].force);
        }
        else if (InDistance(_projectileRange) && !_isMelee)
        {
            _canAttack = false;
            _attackIndex = 0;
            _knockBackVector = EnumLib.KnockbackVector(_enemySkills[_attackIndex].force);
        }
    }

    public virtual void Interrupt()
    {
        Recovery();
    }

    protected void ReadyAttack()
    {
        StartArmor?.Invoke(true);
        _animOverride[ATTACK] = _enemySkills[_attackIndex].animations[0];
        _animOverride[RECOVERY] = _enemySkills[_attackIndex].animations[1];
        
        _anim.Play(ATTACK);
    }

    protected void ChargingAttacks(float windUpTime, float attackDuration)
    {
        StartArmor?.Invoke(true);
        _animOverride[ATTACKCHARGE] = _enemySkills[_attackIndex].animations[0];
        _animOverride[ATTACKRELEASE] = _enemySkills[_attackIndex].animations[1];

        _anim.Play(ATTACKCHARGE);

        _attackWindUp = StartCoroutine(ChargeTimers(new Vector2(windUpTime,attackDuration), true));
    }

    protected virtual IEnumerator IdleTimer(float idleTime)
    {
        yield return new WaitForSeconds(idleTime);
        _canAttack = true;
        _idleTimer = null;
    }

    protected virtual IEnumerator ChargeTimers(Vector2 chargeTime, bool isWindUp)
    {
        
        if (isWindUp)
        {
            yield return new WaitForSeconds(chargeTime.x);
            _anim.SetTrigger("release");
            _attackWindUp = StartCoroutine(ChargeTimers(chargeTime,false));
        }
        else
        {
            if (chargeTime.y > 0f)
            {
                yield return new WaitForSeconds(chargeTime.y);
                _anim.SetTrigger("endAttack");
                _attackWindUp = null;
            }
            else
                yield return null;
        }
    }

    void Update()
    {
        if (_canAttack && _enemySkills.Length > 0)
        {
            SelectAttack();
            if (_attackIndex != -1)
            {
                ReadyAttack();
            }
        }
    }
}