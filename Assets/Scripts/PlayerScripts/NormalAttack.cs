using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttack : MonoBehaviour
{
    [SerializeField]private Animator _anim;
    [SerializeField]private PlayerStats _playerStat;

    int _attackBuffer = 0,_inAttack = 0;
    [SerializeField]int _chain = 0;
    [SerializeField]Collider2D _hurtBox;
    [SerializeField]ContactFilter2D _hurtLayers;
    public AnimatorOverrideController aoc;

    private Vector2 _knockBackVector = Vector2.zero;

    EnumLib.DamageType _activeDamageType = EnumLib.DamageType.Neutral;

    [SerializeField]private List<PlayerAbility> _broomNormalAttacks = new List<PlayerAbility>(3);
    [SerializeField]private List<PlayerAbility> _mopNormalAttacks = new List<PlayerAbility>(3);

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
        _anim.runtimeAnimatorController = aoc;
        
    }

    void Recover()
    {
        if (_attackBuffer != 0)
        {
            Attack(_attackBuffer);
        }
        else
        {
            Debug.Log("End Chain");
            _inAttack = 0;
            _chain = 0;
        }
        _attackBuffer = 0;
    }


    void Hit()
    {
        List<Collider2D> targets = new List<Collider2D>();
        _hurtBox.gameObject.SetActive(true);
        Physics2D.OverlapCollider(_hurtBox,_hurtLayers,targets);
        _hurtBox.gameObject.SetActive(false);
        bool didHit = false;


        DamageEffect damageEffect;
        EnemyStats _enemyStat;

        if (targets.Count != 0)
        {
            foreach(Collider2D col in targets)
            {
                damageEffect = col.gameObject.GetComponent<DamageEffect>();

                if (damageEffect != null && col.CompareTag("Enemy"))
                {
                    _enemyStat = col.gameObject.GetComponent<EnemyStats>();
                    _enemyStat.DamageCalc(_playerStat.attack,_activeDamageType,false);
                    if (_enemyStat._healthRatio > 0f)
                    {
                        damageEffect.TriggerEffect(_inAttack);
                        Vector2 direction = (col.transform.position - transform.position).normalized;
                        col.gameObject.GetComponent<EnemyInterrupt>().Stagger(_inAttack, direction * _knockBackVector);
                    }
                    didHit = true;
                }
                    
            }
        }

        if (didHit)
        {
            // Question mark acts as a null check to avoid invoking an action if not initialized somehow
            CameraFollow.StartShake?.Invoke();
        }
    }

    void Attack(int weapon)
    {
        if (weapon == 1)
        {
            aoc["Attack"] = _broomNormalAttacks[_chain].animations[0];
            aoc["Recovery"] = _broomNormalAttacks[_chain].animations[1];
            _knockBackVector = EnumLib.KnockbackVector(_broomNormalAttacks[_chain].force);
        }
        else if (weapon == 2)
        {
            aoc["Attack"] = _mopNormalAttacks[_chain].animations[0];
            aoc["Recovery"] = _mopNormalAttacks[_chain].animations[1];
             _knockBackVector = EnumLib.KnockbackVector(_mopNormalAttacks[_chain].force);
        }

        _activeDamageType = (EnumLib.DamageType)weapon;

        Debug.Log("Current Damage Type is "+_activeDamageType.ToString());
        
        _anim.Play("Attack");

        _chain = (_chain + 1) % _broomNormalAttacks.Count;

        _inAttack = weapon;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp("z") && _attackBuffer == 0)
        {
            if (_inAttack == 0)
            {
                Attack(1);
            }
            else
            {
                _attackBuffer = 1;
            }
                
        }

        if (Input.GetKeyUp("x") && _attackBuffer == 0)
        {
            if (_inAttack == 0)
            {
                Attack(2);
            }
            else
            {
                _attackBuffer = 2;
            }
                
        }
    }
}
