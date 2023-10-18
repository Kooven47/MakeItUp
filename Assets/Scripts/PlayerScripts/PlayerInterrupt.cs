using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInterrupt : InterruptSystem
{
    private PlayerStats _playerStats;

    public static Action<bool> staggered;

    [SerializeField]private float _invincTimer = 0.5f;

    private Coroutine _iFrameTimer;

    public bool iFrame
    {
        get{return _iFrameTimer != null;}
    }

    protected override void Start()
    {
        base.Start();
        _playerStats = GetComponent<PlayerStats>();
    }

    protected override IEnumerator StaggerTime(float staggerTime)
    {
        yield return new WaitForSeconds(staggerTime);
        _anim.SetTrigger("unStagger");
        staggered?.Invoke(true);
        _isStunned = false;
        _staggerTimer = null;
    }

    private IEnumerator InvincibilityTimer(float iFrameTime)
    {
        yield return new WaitForSeconds(iFrameTime);
        _iFrameTimer = null;
    }
    public override void Stagger(int damageType, Vector2 knockVector)
    {
        if (_poise == ArmorType.SuperArmor || iFrame)
            return;
        
        _isStunned = true;
        _rb.AddForce(knockVector * _rb.mass,ForceMode2D.Impulse);

        _anim.Play("Stagger");

        staggered?.Invoke(false);

       _staggerTimer = StartCoroutine(StaggerTime(1f));
       _iFrameTimer = StartCoroutine(InvincibilityTimer(1f + _invincTimer));
    }
}