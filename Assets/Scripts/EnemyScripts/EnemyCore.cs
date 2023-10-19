using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyCore : MonoBehaviour
{
    [SerializeField] private bool _isMelee = true;
    [SerializeField] protected Vector2 _projectileRange = new Vector2(3f,2f);
    [SerializeField] protected Vector2 _meleeRange = new Vector2(3f,2f);
    [SerializeField] ContactFilter2D _hurtLayers;
    [SerializeField] protected Collider2D _hurtBox;
    [SerializeField] protected EnemyAbility[] _enemySkills = new EnemyAbility[2];
    [SerializeField] protected AnimatorOverrideController _animOverride;
    protected const string ATTACK ="Attack", RECOVERY = "Recovery";
    protected Coroutine _idleTimer;

    protected Animator _anim;

    protected bool _canAttack = false;

    protected int _attackIndex = -1;

    [SerializeField]protected Transform _target;

    public Action<bool> StartArmor;

    protected Vector2 _knockBackVector;

    protected virtual void Start()
    {
        _idleTimer = StartCoroutine(IdleTimer(3f));
        _animOverride = Instantiate(_animOverride);
        _anim = GetComponent<Animator>();
        _anim.runtimeAnimatorController = _animOverride;
        _target = GameObject.Find("Janitor").transform;
        _hurtBox = transform.GetChild(0).GetComponent<Collider2D>();
    }

    protected bool InDistance(Vector2 range)
    {
        float x_dist = Mathf.Abs(transform.position.x - _target.position.x);
        float y_dist = Mathf.Abs(transform.position.y - _target.position.y);

        return x_dist <= range.x && y_dist <= range.y;
    }

    protected void ProjectileFire()
    {

    }

    protected void MeleeStrike()
    {
        List<Collider2D> targets = new List<Collider2D>();
        _hurtBox.gameObject.SetActive(true);
        Physics2D.OverlapCollider(_hurtBox,_hurtLayers,targets);
        _hurtBox.gameObject.SetActive(false);
        bool didHit = false;

        PlayerInterrupt _playerInter;

        if (targets.Count != 0)
        {
            foreach(Collider2D col in targets)
            {
                if (col.CompareTag("Player"))
                {
                    Debug.Log("Hit the janitor!");
                    _playerInter = col.GetComponent<PlayerInterrupt>();
                    if (!_playerInter.iFrame)
                    {
                        Vector2 direction = (col.transform.position - transform.position).normalized;
                        _playerInter.Stagger(1,_knockBackVector * direction * 0.5f);
                        col.GetComponent<PlayerStats>().DamageCalc(_enemySkills[_attackIndex].damage,_enemySkills[_attackIndex].attribute,false);
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

    protected void Recovery()
    {
        _canAttack = false;
        StartArmor?.Invoke(false);
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
    }

    public void Interrupt()
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

    protected virtual IEnumerator IdleTimer(float idleTime)
    {
        yield return new WaitForSeconds(idleTime);
        _canAttack = true;
        _idleTimer = null;
    }

    void Update()
    {
        if (_canAttack)
        {
            SelectAttack();
            if (_attackIndex != -1)
            {
                ReadyAttack();
            }
        }
    }


}