using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DirtyDishCore : EnemyCore
{
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _firePointOffset = 0.2f;
    protected override void ProjectileFire()
    {
        int skillIndex = _attackIndex;
        if (skillIndex == -1)
            skillIndex = 0;
        
        ProjectileManager.createProjectile(new Vector2(_firePoint.position.x,_firePoint.position.y - _firePointOffset),Direction(),_enemySkills[0],_enemyStats.attack);
        ProjectileManager.createProjectile(_firePoint.position,Direction(),_enemySkills[0],_enemyStats.attack);
        ProjectileManager.createProjectile(new Vector2(_firePoint.position.x,_firePoint.position.y + _firePointOffset),Direction(),_enemySkills[0],_enemyStats.attack);
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