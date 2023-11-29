using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : Stats, ISaveGame
{
    [SerializeField] private float _critRate = 0.05f, _critDMG = 0.5f;
    [SerializeField] private float _invincTimer = 0.5f;
    private Coroutine _iFrameTimer;
    public static bool playerIsDead = false;
    public static Action<float> dashIFrame;
    PlayerControllerJanitor _playerControllerJanitor;

    private bool _loadedStats = false;

    public bool iFrame
    {
        get {return _iFrameTimer != null;}
    }
    // Start is called before the first frame update
    void Start()
    {
        _playerControllerJanitor = GameObject.FindWithTag("Player").GetComponent<PlayerControllerJanitor>();
        
        if(!_loadedStats)
            base.Start();
        
        HealthBar.settingHealth?.Invoke(_curHP,_maxHP);
        playerIsDead = false;
        dashIFrame = DashIFrame;
    }

    private IEnumerator InvincibilityTimer(float iFrameTime)
    {
        yield return new WaitForSeconds(iFrameTime);
        _iFrameTimer = null;
    }

    public override void DamageCalc(float attack, EnumLib.DamageType attribute, bool isCrit)
    {
        float damage = (attack) * (isCrit ? 1.5f : 1.0f);
        curHP -= damage;
        const int GOTHIT = 5;
        _playerControllerJanitor.PlaySoundEffect(GOTHIT);

        DamageNumberPool.summonDamageNum?.Invoke(damage,0,transform.position);
        HealthBar.settingHealth?.Invoke(_curHP,_maxHP);
        _iFrameTimer = StartCoroutine(InvincibilityTimer(1f + _invincTimer));
        
        if (_curHP <= 0) Death();
    }

    public void DashIFrame(float iFrameLength)
    {
        if (_iFrameTimer != null) StopCoroutine(_iFrameTimer);
        _iFrameTimer = StartCoroutine(InvincibilityTimer(iFrameLength));
    }

    public void RestoreHealth(float percent)
    {
        curHP += _maxHP * percent;
        HealthBar.settingHealth?.Invoke(_curHP,_maxHP);
    }

    public override void Death()
    {
        GameOverMenu.gameOver?.Invoke();
    }

    public void LoadSaveData(SaveData data)
    {
        if (data.janitorStartMaxHealth < 0f && data.janitorMaxHealth < 0f)
        {
            Debug.Log("Fresh save");
            base.Start();
            SaveInitialData(ref data);
            return;
        }

        // _maxHP = data.janitorMaxHealth;
        // _curHP = data.janitorCurrentHealth;
        // _loadedStats = true;
    }

    public void LoadInitialData(SaveData data)
    {
        // _maxHP = data.janitorStartMaxHealth;
        // _curHP = data.janitorStartCurrentHealth;

        // data.janitorMaxHealth = data.janitorStartMaxHealth;
        // data.janitorCurrentHealth = data.janitorStartCurrentHealth;
    }

    public void SaveData(ref SaveData data)
    {
        // if (_curHP <= 0f) return;
        // data.janitorMaxHealth = _maxHP;
        // data.janitorCurrentHealth = _curHP;
    }

    public void SaveInitialData(ref SaveData data)
    {
        // data.janitorStartMaxHealth = _maxHP;
        // data.janitorStartCurrentHealth = _curHP;
    }
}
