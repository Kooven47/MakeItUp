using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInterrupt : InterruptSystem
{
    private PlayerStats _playerStats;

    public static Action<bool> staggered;

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
    public override void Stagger(int damageType, Vector2 knockVector)
    {
        if (_poise == ArmorType.SuperArmor)
            return;
        
        _isStunned = true;
        _rb.AddForce(knockVector * _rb.mass,ForceMode2D.Impulse);

        _anim.Play("Stagger");

        staggered?.Invoke(false);

       _staggerTimer = StartCoroutine(StaggerTime(1f));
    }
}