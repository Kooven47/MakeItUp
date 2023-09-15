using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttack : MonoBehaviour
{
    [SerializeField]private Animator _anim;
    bool _attackBuffer = false, _inAttack = false;
    [SerializeField]int _chain = 0;
    [SerializeField]Collider2D _hurtBox;
    [SerializeField]ContactFilter2D _hurtLayers;
    public AnimatorOverrideController aoc;

    [SerializeField]private List<PlayerAbility> _normalAttacks = new List<PlayerAbility>(3);

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
        _anim.runtimeAnimatorController = aoc;
        
    }

    void Recover()
    {
        if (_attackBuffer)
        {
            _attackBuffer = false;
            Attack();
        }
        else
        {
            Debug.Log("End Chain");
            _inAttack = false;
            _chain = 0;
        }
    }


    void Hit()
    {
        List<Collider2D> targets = new List<Collider2D>();
        _hurtBox.gameObject.SetActive(true);
        Physics2D.OverlapCollider(_hurtBox,_hurtLayers,targets);
        _hurtBox.gameObject.SetActive(false);
        bool didHit = false;

        DamageEffect damageEffect;

        if (targets.Count != 0)
        {
            foreach(Collider2D col in targets)
            {
                Debug.Log("Hit "+col.name);
                damageEffect = col.gameObject.GetComponent<DamageEffect>();

                if (damageEffect != null && col.CompareTag("Enemy"))
                {
                    damageEffect.TriggerEffect();
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

    void Attack()
    {
        
        aoc["Attack"] = _normalAttacks[_chain].animations[0];
        aoc["Rec"] = _normalAttacks[_chain].animations[1];
        _anim.Play("Attack");

        _chain = (_chain + 1) % _normalAttacks.Count;

        _inAttack = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp("z"))
        {
            if (!_inAttack)
            {
                Attack();
            }
            else
            {
                _attackBuffer = true;
            }
                
        }
    }
}
