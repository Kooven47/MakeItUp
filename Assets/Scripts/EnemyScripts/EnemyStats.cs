using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyStats : Stats
{
    public const int RESIST = -1, NEUTRAL = 0, WEAK = 1;
    [SerializeField]EnumLib.DamageType _attribute = EnumLib.DamageType.Neutral;
    private float _shieldHealth = 0f;
    public bool isShielded {get {return _isShielded;} private set {_isShielded = value;}}

    private bool _isShielded = false;
    EnumLib.DamageType _shieldAttribute = EnumLib.DamageType.Neutral;
    public static event Action OnDeath;
    // Start is called before the first frame update
    protected void Start()
    {
        base.Start();
    }

    public int IsEffective(EnumLib.DamageType attribute)
    {
        if (_attribute == attribute)
        {
            return WEAK;
        }
        else if (_attribute != EnumLib.DamageType.Neutral)
        {
            return RESIST;
        }

        return NEUTRAL;
    }

    public override void Death()
    {
         Debug.Log("DEFEATED!");
            OnDeath?.Invoke();
            Destroy(gameObject);
        // this.gameObject.SetActive(false);
    }

    public override void DamageCalc(float attack,EnumLib.DamageType attribute, bool isCrit)
    {
        float damage = (attack - _defense/2f) * (isCrit ? 1.0f : 1.5f);
        int effective = 0;
        Debug.Log("Attribute is "+attribute);

        switch(IsEffective(attribute))
        {
            case RESIST:
                Debug.Log("RESIST!");
                damage *= 0.5f;
                effective = -1;
            break;

            case WEAK:
                damage *= 1.5f;
                effective = 1;
            break;
        }

        _curHP -= damage;
        Debug.Log("Damage received "+damage);

        DamageNumberPool.summonDamageNum?.Invoke(damage,effective,transform.position);

        if (_curHP <= 0f)
        {
            Death();
        }
    }

}
