using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : Stats
{
    [SerializeField] private float _critRate = 0.05f, _critDMG = 0.5f;
    [SerializeField] private float _invincTimer = 0.5f;
    private Coroutine _iFrameTimer;
    public static bool playerIsDead = false;

    public bool iFrame
    {
        get {return _iFrameTimer != null;}
    }
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        HealthBar.settingHealth?.Invoke(_curHP,_maxHP);
        playerIsDead = false;
    }

    private IEnumerator InvincibilityTimer(float iFrameTime)
    {
        yield return new WaitForSeconds(iFrameTime);
        _iFrameTimer = null;
    }

    public override void DamageCalc(float attack, EnumLib.DamageType attribute, bool isCrit)
    {
        float damage = (attack - _defense / 2f) * (isCrit ? 1.5f : 1.0f);
        _curHP -= damage;

        DamageNumberPool.summonDamageNum?.Invoke(damage,0,transform.position);
        HealthBar.settingHealth?.Invoke(_curHP,_maxHP);
        _iFrameTimer = StartCoroutine(InvincibilityTimer(1f + _invincTimer));
        if (_curHP <= 0) Death();
    }

    public override void Death()
    {
        playerIsDead = true;
    }
}
