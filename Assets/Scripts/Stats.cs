using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;
using System;

public class Stats : MonoBehaviour
{
    [SerializeField]protected float _maxHP = 50f,_attack = 5,_defense = 2;
    [SerializeField]protected float _moveSpeed = 1.0f;
    protected float _curHP = 50f;
    // Start is called before the first frame update

    public float healthRatio
    {
        get {return _curHP/_maxHP;}
    }
    protected void Start()
    {
        _curHP = _maxHP;
    }

    public float attack
    {
        get {return _attack;}
    }

    public float defense
    {
        get {return _defense;}
    }

    protected float curHP
    {
        get{return _curHP;}
        set{
            _curHP = value;
            if (curHP > _maxHP)
                _curHP = _maxHP;
            else if (curHP < 0f)
            {
                _curHP = 0f;
            }
        }
    }

    public virtual void DamageCalc(float attack, EnumLib.DamageType attribute ,bool isCrit)
    {
        
    }

    public virtual void Death()
    {
        Debug.Log("DEFEATED!");
        Destroy(gameObject, 1.0f);
    }

}
