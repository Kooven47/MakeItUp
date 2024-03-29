using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyStats : Stats
{
    public const int RESIST = -1, NEUTRAL = 0, WEAK = 1;
    [SerializeField] EnumLib.DamageType _attribute = EnumLib.DamageType.Neutral;
    [SerializeField] private float _healthDropChance = 0.25f;

    public EnumLib.DamageType Attribute
    {
        get => _attribute;
        set => _attribute = value;
    }

    private float _shieldHealth = 0f;
    public bool isShielded {get {return _isShielded;} private set {_isShielded = value;}}

    private bool _isShielded = false;
    [SerializeField] private bool _isBoss = false;
    EnumLib.DamageType _shieldAttribute = EnumLib.DamageType.Neutral;
    public static event Action OnDeath;
    public static Action <EnumLib.DamageType> OnDeathWithType;
    public static event Action BossOnDeath;

    public bool isBoss
    {
        get{return _isBoss;}
    }

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
        if (_isBoss)
        {
            BossOnDeath?.Invoke();
        }
        else
        {
            OnDeath?.Invoke();
            OnDeathWithType?.Invoke(_attribute);
        }
        Destroy(gameObject);
        if (PlayerStatus.Instance.playerHPRatio <= 0.5f)
        {
            if (UnityEngine.Random.Range(0f,1f) <= _healthDropChance)
            {
                PowerupManager.spawnHealingItem?.Invoke(0, transform.position);
            }
            else
            {
                Debug.Log("Better luck next time for health drop!");
            }
        }
        else
        {
            Debug.Log("HP Ratio of player is "+PlayerStatus.Instance.playerHPRatio);
        }
        // this.gameObject.SetActive(false);
    }

    public override void DamageCalc(float attack,EnumLib.DamageType attribute, bool isCrit)
    {
        float damage = attack;
        int effective = 0;
        Debug.Log("Attribute is "+attribute);

        switch(IsEffective(attribute))
        {
            case RESIST:
                Debug.Log("RESIST!");
                damage *= 0.5f;
                effective = -1;
                isCrit = false;
            break;

            case WEAK:
                damage *= 1.5f;
                effective = 1;
            break;
        }

        damage *= (isCrit ? 1.5f : 1.0f);
        _curHP -= damage;
        Debug.Log("Damage received "+damage);

        DamageNumberPool.summonDamageNum?.Invoke(damage,effective,transform.position,isCrit);

        if(_isBoss)
            BossHealthBar.settingHealth?.Invoke(_curHP,_maxHP);

        if (_curHP <= 0f)
        {
            Death();
        }
    }

}
