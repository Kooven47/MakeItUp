using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCore : MonoBehaviour
{
    [SerializeField] protected BoxCollider2D _toiletCollider;
    [SerializeField] protected List<EnemyAbility> _moveSet = new List<EnemyAbility>();
    [SerializeField] protected AnimatorOverrideController _animOverride;
    protected Rigidbody2D _rb;
    protected EnemyStats _bossStats;
    protected Animator _anim;

    protected int _curPhase = 0;

    protected float[] _toNextPhase = new float[1];// HP Thresholds

    protected virtual void Start()
    {
        _anim = transform.GetChild(0).GetComponent<Animator>();
        transform.GetChild(0).GetComponent<AnimFunctions>().bossCore = GetComponent<BossCore>();
        if (_animOverride != null)
        {
            _animOverride = Instantiate(_animOverride);
            _anim.runtimeAnimatorController = _animOverride;
        }
        else
            Debug.LogError("Animator Override in BossCore not set in "+gameObject.name);
    }

    protected virtual void Flip()
    {

    }

    public virtual void Fire()
    {

    }

    public virtual void SetUpAttack(int i)
    {
        _animOverride["Attack"] = _moveSet[i].animations[0];
        _animOverride["Recovery"] = _moveSet[i].animations[1];
        _anim.Play("Attack");
    }
    
    public virtual void Recovery()
    {
        // _anim.SetTrigger("recover");
    }
}