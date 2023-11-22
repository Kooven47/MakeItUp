using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DamageContact : MonoBehaviour
{
    protected BoxCollider2D _collider;
    protected bool _isDamageContact = false;

    protected Ability _skill;

    public Ability skill
    {
        get{return _skill;}
        set{_skill = value;}
    }

    public void StartAttack()
    {
        _isDamageContact = true;
    }

    public void EndAttack()
    {
        _isDamageContact = false;
    }

    protected virtual void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
    }

    protected virtual void DealDamage(Collision2D collider)
    {

    }

    protected void OnCollisionEnter2D(Collision2D col)
    {
        
        if (_isDamageContact)
            DealDamage(col);
        else
        {
            Debug.Log("Contact damage is toggled "+_isDamageContact);
        }
    }

}