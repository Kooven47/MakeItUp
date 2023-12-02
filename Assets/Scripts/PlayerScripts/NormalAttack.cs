using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class NormalAttack : MonoBehaviour
{
    [SerializeField]private Animator _anim;
    [SerializeField]private PlayerStats _playerStat;
    [SerializeField]private float _chainDMGBoost = 0.25f;

    [SerializeField]int _attackBuffer = 0,_inAttack = 0, _useFu = 0;
    [SerializeField]int _chain = 0;
    [SerializeField]Collider2D _hurtBox;
    [SerializeField]ContactFilter2D _hurtLayers;
    public AnimatorOverrideController aoc;

    private Vector2 _knockBackVector = Vector2.zero;

    private enum Direction {UP = 0,SIDE = 1,DOWN = 1};

    EnumLib.DamageType _activeDamageType = EnumLib.DamageType.Neutral;

    float _damageDealing = 0f,_chainDMGMod = 1f;

    [SerializeField]private List<PlayerAbility> _broomNormalAttacks = new List<PlayerAbility>(3);
    [SerializeField]private List<PlayerAbility> _mopNormalAttacks = new List<PlayerAbility>(3);
    [SerializeField]private List<PlayerAbility> _broomFu = new List <PlayerAbility>();
    [SerializeField]private List<PlayerAbility> _mopFu = new List <PlayerAbility>();

    [SerializeField]private bool _canAttack = true;

    private PlayerControllerJanitor _playerControllerJanitor;

    // Start is called before the first frame update
    void Start()
    {
        _playerControllerJanitor = GameObject.FindWithTag("Player").GetComponent<PlayerControllerJanitor>();
        _anim = GetComponent<Animator>();
        _anim.runtimeAnimatorController = aoc;
        PlayerInterrupt.staggered += SetCanAttack;
    }

    public void PlayDeathAnim()
    {
        Debug.Log("Called player death anim");
        _anim.Play("Defeat");
        _anim.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    public void SetCanAttack(bool toggle)
    {
        _canAttack = toggle;
        if (toggle)
        {
            _attackBuffer = 0;
            _inAttack = 0;
            _chain = 0;
            _useFu = 0;
        }
    }

    void Recover()
    {
        if (_attackBuffer != 0)
        {
            if (_useFu == 0)
                Attack(_attackBuffer);
            else
            {
                if (_useFu == 1)
                {
                    JanitorFu(_attackBuffer,(int)Direction.UP);
                }
                else if (_useFu == 2)
                {
                    JanitorFu(_attackBuffer,(int)Direction.DOWN);
                }
            }
        }
        else
        {
            Debug.Log("End Chain");
            _inAttack = 0;
            _useFu = 0;
            _chain = 0;
        }
        _attackBuffer = 0;
        _useFu = 0;
    }


    void Hit()
    { 
        List<Collider2D> targets = new List<Collider2D>();
        _hurtBox.gameObject.SetActive(true);
        Physics2D.OverlapCollider(_hurtBox,_hurtLayers,targets);
        _hurtBox.gameObject.SetActive(false);
        bool didHit = false, didCrit = false;
        
        DamageEffect damageEffect;
        EnemyStats _enemyStat;
        ProjectileScript _projectile;

        if (targets.Count != 0)
        {
            foreach(Collider2D col in targets)
            {
                damageEffect = col.gameObject.GetComponent<DamageEffect>();

                if (damageEffect != null && col.CompareTag("Enemy"))
                {
                    _enemyStat = col.gameObject.GetComponent<EnemyStats>();
                    didCrit = _enemyStat.Attribute == EnumLib.DamageType.Neutral ? _playerStat.DidCritical() : _playerStat.DidCriticalEnhanced();
                    _enemyStat.DamageCalc(_damageDealing * _chainDMGMod, _activeDamageType, didCrit);
                    if (_enemyStat.healthRatio > 0f)
                    {
                        damageEffect.TriggerEffect(_inAttack);
                        Vector2 direction = (Vector2)(col.transform.position - transform.position).normalized;
                        direction.Set(direction.x,1f);
                        col.gameObject.GetComponent<EnemyInterrupt>().Stagger((int)_activeDamageType, direction * _knockBackVector);
                    }
                    didHit = true;
                }
                else if (col.CompareTag("Projectile"))
                {
                    _projectile = col.GetComponent<ProjectileScript>();
                    if ((int)_projectile.damageType == _inAttack)
                    {
                        _projectile.Dissipate();
                        Debug.Log("Destroyed projectile");
                    }
                    else
                    {
                        Debug.Log("Wrong weapon to destroy");
                    }
                }
            }
        }

        const int WET = 6, DRY = 7, MISS = 8;
        if (didHit)
        {
            if ((int)_activeDamageType == 1)
            {
                _playerControllerJanitor.PlaySoundEffect(DRY);
            }
            else
            {
                _playerControllerJanitor.PlaySoundEffect(WET);
            }
            // Question mark acts as a null check to avoid invoking an action if not initialized somehow
            CameraFollow.StartShake?.Invoke();
        }
        else
        { 
            _playerControllerJanitor.PlaySoundEffect(MISS);
        }
    }

    void JanitorFu(int weapon, int direction)
    {
        if (weapon == 1)
        {
            if (direction < _broomFu.Count)
            {
                aoc["Attack"] = _broomFu[direction].animations[0];
                aoc["Recovery"] = _broomFu[direction].animations[1];
                _knockBackVector = EnumLib.KnockbackVector(_broomFu[direction].force);
                _damageDealing = _broomFu[direction].damage;
                _anim.Play("Attack");
                _inAttack = weapon;
                _activeDamageType = (EnumLib.DamageType)weapon;
                Debug.Log("Broom Skill in Attack "+((EnumLib.DamageType)_inAttack));
            }
        }
        else if (weapon == 2)
        {
            if (direction < _mopFu.Count)
            {
                aoc["Attack"] = _mopFu[direction].animations[0];
                aoc["Recovery"] = _mopFu[direction].animations[1];
                _damageDealing = _mopFu[direction].damage;
                _knockBackVector = EnumLib.KnockbackVector(_mopFu[direction].force);
                _anim.Play("Attack");
                _inAttack = weapon;
                _activeDamageType = (EnumLib.DamageType)weapon;
                Debug.Log("Mop Skill in Attack "+((EnumLib.DamageType)_inAttack));
            }
        }
        _chainDMGMod = 1 + (_chainDMGBoost * _chain);
        _chain = (_chain + 1) % _broomNormalAttacks.Count;
    }

    void Attack(int weapon)
    {
        if (weapon == 1)
        {
            aoc["Attack"] = _broomNormalAttacks[_chain].animations[0];
            aoc["Recovery"] = _broomNormalAttacks[_chain].animations[1];
            _knockBackVector = EnumLib.KnockbackVector(_broomNormalAttacks[_chain].force);
            _damageDealing = _broomNormalAttacks[_chain].damage;
        }
        else if (weapon == 2)
        {
            aoc["Attack"] = _mopNormalAttacks[_chain].animations[0];
            aoc["Recovery"] = _mopNormalAttacks[_chain].animations[1];
            _damageDealing = _mopNormalAttacks[_chain].damage;
            _knockBackVector = EnumLib.KnockbackVector(_mopNormalAttacks[_chain].force);
        }

        _activeDamageType = (EnumLib.DamageType)weapon;

        Debug.Log("Current Damage Type is "+_activeDamageType.ToString());
        
        _anim.Play("Attack");

        _chainDMGMod = 1 + (_chainDMGBoost * _chain);

        _chain = (_chain + 1) % _broomNormalAttacks.Count;

        _inAttack = weapon;
    }

    // Update is called once per frame
    void Update()
    {   
        if (Time.timeScale != 0 && _attackBuffer == 0 && _canAttack)
        {
            if (Input.GetKeyUp("j") && _attackBuffer == 0)
            {
                if (Input.GetAxisRaw("Vertical") > 0f)
                {
                    if (_useFu == 0 && _inAttack == 0)
                        JanitorFu(1,(int)Direction.UP);
                    else
                    {
                        _useFu = 1;
                        _attackBuffer = 1;
                    }
                }
                else if (Input.GetAxisRaw("Vertical") < 0f)
                {
                    if (_useFu == 0 && _inAttack == 0)
                        JanitorFu(1,(int)Direction.DOWN);
                    else
                    {
                        _useFu = 2;
                        _attackBuffer = 1;
                    }
                }
                else
                {
                    if (_inAttack == 0)
                        Attack(1);
                    else
                    {
                        _attackBuffer = 1;
                    }
                }
                // if (_inAttack == 0)
                // {
                    
                // }
                // else
                // {
                //     _attackBuffer = 1;
                // }
            }

            if (Input.GetKeyUp("k") && _attackBuffer == 0)
            {
                if (Input.GetAxisRaw("Vertical") > 0f)
                {
                    if (_useFu == 0 && _inAttack == 0)
                        JanitorFu(2,(int)Direction.UP);
                    else
                    {
                        _useFu = 1;
                        _attackBuffer = 2;
                    }
                }
                else if (Input.GetAxisRaw("Vertical") < 0f)
                {
                    if (_useFu == 0 && _inAttack == 0)
                        JanitorFu(2,(int)Direction.DOWN);
                    else
                    {
                        _useFu = 2;
                        _attackBuffer = 2;
                    }
                }
                else
                {
                    if (_inAttack == 0)
                        Attack(2);
                    else
                    {
                        _attackBuffer = 2;
                    }
                }
            }
        }
    }
}
