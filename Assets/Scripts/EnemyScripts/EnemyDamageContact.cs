using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyDamageContact : DamageContact
{
    protected override void DealDamage(Collision2D collider)
    {
        Collider2D col = collider.collider;
        Debug.Log("Calling damagecontact");
        if (col.CompareTag("Player") && _skill != null)
        {
            bool didHit = false;
            PlayerStats _playerStat = col.GetComponent<PlayerStats>();
            if (!_playerStat.iFrame)
            {
                Vector2 direction = (col.transform.position - transform.position).normalized;
                col.GetComponent<PlayerInterrupt>().Stagger(1,EnumLib.KnockbackVector(_skill.force) * direction * 0.5f);
                _playerStat.DamageCalc(_skill.damage,_skill.attribute,false);
                col.gameObject.GetComponent<DamageEffect>().TriggerEffect((int)_skill.attribute);
                didHit = true;
            }
            else
            {
                Debug.Log("Under Iframes");
            }
            if (didHit)
            {
                // Question mark acts as a null check to avoid invoking an action if not initialized somehow
                CameraFollow.StartShake?.Invoke();
            }
        }

        _isDamageContact = false;
    }
}