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

}
