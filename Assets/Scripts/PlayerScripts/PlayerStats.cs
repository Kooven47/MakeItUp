using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : Stats
{
    [SerializeField] private float _critRate = 0.05f, _critDMG = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        HealthBar.settingHealth?.Invoke(_curHP,_maxHP);
    }

    public override void DamageCalc(float attack, EnumLib.DamageType attribute ,bool isCrit)
    {
        float damage = (attack - _defense/2f) * (isCrit ? 1.0f : 1.5f);
        _curHP -= damage;

        DamageNumberPool.summonDamageNum?.Invoke(damage,0,transform.position);
        HealthBar.settingHealth?.Invoke(_curHP,_maxHP);
    }

}
