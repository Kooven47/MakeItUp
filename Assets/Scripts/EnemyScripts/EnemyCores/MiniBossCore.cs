using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Mathematics;

public class MiniBossCore: EnemyCore
{
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _firePointOffset = 0.2f;
    protected override void Start()
    {
        base.Start();
        _enemySkills[0] = Instantiate(_enemySkills[0]);
        _enemySkills[0].attribute = _enemyStats.Attribute;
    }

    protected override void ProjectileFire()
    {
        int skillIndex = _attackIndex;
        if (skillIndex == -1)
            skillIndex = 0;
        
        ProjectileManager.createHoming?.Invoke(_firePoint.position,_target,_enemySkills[skillIndex],_enemyStats.attack);
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