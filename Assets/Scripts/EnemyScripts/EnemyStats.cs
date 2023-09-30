using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : Stats
{
    [SerializeField]EnumLib.DamageType _attribute = EnumLib.DamageType.Neutral;
    // Start is called before the first frame update
    protected void Start()
    {
        base.Start();
    }

    public override void DamageCalc(float attack,EnumLib.DamageType attribute, bool isCrit)
    {
        float damage = (attack - _defense/2f) * (isCrit ? 1.0f : 1.5f);
        int effective = 0;
        if (_attribute == attribute)
        {
            Debug.Log("Weakness!");
            damage *= 1.5f;
            effective = 1;
        }
        else if (_attribute != EnumLib.DamageType.Neutral)
        {
            Debug.Log("RESIST!");
            damage *= 0.5f;
            effective = -1;
        }
        _curHP -= damage;

        Debug.Log("Damage received "+damage);

        DamageNumberPool.summonDamageNum?.Invoke(damage,effective,transform.position);
    }

}
