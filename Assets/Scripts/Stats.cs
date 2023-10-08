using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    [SerializeField]protected float _maxHP = 50f,_attack = 5,_defense = 2;
    [SerializeField]protected float _moveSpeed = 1.0f;
    protected float _curHP;
    // Start is called before the first frame update
    protected void Start()
    {
        _curHP = _maxHP;
    }

    public float attack
    {
        get{return _attack;}
    }

    public float defense
    {
        get{return _defense;}
    }

    public virtual void DamageCalc(float attack, EnumLib.DamageType attribute ,bool isCrit)
    {
        
    }

    public virtual void Death()
    {
        
    }

}
