using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInterrupt : InterruptSystem
{
    private EnemyStats _enemyStats;

    protected override void Start()
    {
        base.Start();
        _enemyStats = GetComponent<EnemyStats>();
    }

    protected override IEnumerator StaggerTime(float staggerTime)
    {
        yield return new WaitForSeconds(staggerTime);
        _anim.SetBool("isStaggered",false);
    }

    public override void Stagger(int damageType, Vector2 knockVector)
    {
        if (_poise == ArmorType.SuperArmor || _enemyStats.isShielded)
            return;

        int effective = _enemyStats.IsEffective((EnumLib.DamageType)damageType);
        float staggerTimeMod = 1f;

        if (effective == -1)
        {
            knockVector *= 0.5f;
            staggerTimeMod = 0.5f;
        }
        else if (effective == 1)
        {
            staggerTimeMod = 1.2f;
        }

        _rb.AddForce(knockVector * _rb.mass,ForceMode2D.Impulse);

        _anim.SetBool("isStaggered",true);

        if (_staggerTimer != null)
        {
            StopCoroutine(_staggerTimer);
        }

       _staggerTimer = StartCoroutine(StaggerTime(1f * staggerTimeMod));

    }
}