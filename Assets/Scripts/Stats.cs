using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    [SerializeField]protected float _maxHP = 50f,_attack = 5,_defense = 2;
    protected float _curHP;
    // Start is called before the first frame update
    protected void Start()
    {
        _curHP = _maxHP;
    }

}
