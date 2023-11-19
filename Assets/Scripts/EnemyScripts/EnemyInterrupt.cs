using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyInterrupt : InterruptSystem
{
    private EnemyStats _enemyStats;
    [SerializeField]private EnemyCore _enemyCore;
    [SerializeField]private bool _canKnockBack = true;

    protected override void Start()
    {
        base.Start();
        Debug.Log("Calling child start");
        _enemyStats = GetComponent<EnemyStats>();
        _enemyCore = transform.GetChild(0).GetComponent<EnemyCore>();
        _enemyCore.StartArmor += SuperArmor;
    }

    protected override IEnumerator StaggerTime(float staggerTime)
    {
        yield return new WaitForSeconds(staggerTime);
        _anim.SetBool("isStaggered",false);
    }

    protected void SuperArmor(bool isSuper)
    {
        _poise = (isSuper ? ArmorType.SuperArmor : ArmorType.Neutral);
    }

    public override void Stagger(int damageType, Vector2 knockVector)
    {
        int effective = _enemyStats.IsEffective((EnumLib.DamageType)damageType);

        if ((_poise == ArmorType.SuperArmor && effective != 1) || _enemyStats.isShielded)
            return;

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
        
        if (_canKnockBack)
            _rb.AddForce(knockVector * _rb.mass,ForceMode2D.Impulse);

        _anim.Play("Stagger");

        _poise = ArmorType.Neutral;
        _enemyCore.Interrupt();

        if (_staggerTimer != null)
        {
            StopCoroutine(_staggerTimer);
        }
        
        _staggerTimer = StartCoroutine(StaggerTime(1f * staggerTimeMod));

    }
}