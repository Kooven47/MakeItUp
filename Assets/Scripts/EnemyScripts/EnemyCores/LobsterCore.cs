using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobsterCore : EnemyCore
{
    private PlayerControllerJanitor _playerContJan;
    private Rigidbody2D _playerRigid;
    
    private bool _lockedPlayer = false;

    protected override void Start()
    {
        base.Start();
        EnemyStats.OnDeath += Recovery;
    }

    public void OnDestroy()
    {
        EnemyStats.OnDeath -= Recovery;
    }
    protected override void Recovery()
    {
        if (_playerContJan != null)
        {
            _playerContJan.SetStunned(false);
            _playerContJan = null;
            _playerRigid.isKinematic = false;
            _playerRigid = null;
            _lockedPlayer = false;
        }
        base.Recovery();
        
    }

    protected override IEnumerator ChargeTimers(Vector2 chargeTime, bool isWindUp)
    {
        
        if (isWindUp)
        {
            yield return new WaitForSeconds(chargeTime.x);
            _anim.SetTrigger("release");
            _attackWindUp = StartCoroutine(ChargeTimers(chargeTime,false));
        }
        else
        {
            float pinchTimer = chargeTime.y;
            while (pinchTimer > 0f)
            {
                if (_lockedPlayer)
                {
                    _playerRigid.position = _hurtBox.transform.position;
                }
                pinchTimer -= Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }
            
            Recovery();
            _anim.SetTrigger("endAttack");
            _attackWindUp = null;
            
        }
    }

    protected override void MeleeStrike()
    {
        List<Collider2D> targets = new List<Collider2D>();
        _hurtBox.gameObject.SetActive(true);
        Physics2D.OverlapCollider(_hurtBox,_hurtLayers,targets);
        _hurtBox.gameObject.SetActive(false);
        bool didHit = false, lockedOn = false;

        PlayerStats _playerStat;
        

        if (targets.Count != 0)
        {
            foreach(Collider2D col in targets)
            {
                if (col.CompareTag("Player"))
                {
                    _playerStat = col.GetComponent<PlayerStats>();
                    _playerContJan = col.GetComponent<PlayerControllerJanitor>();
                    if (!_playerStat.iFrame)
                    {
                        if (_playerContJan.SetStunned(true))
                        {
                            lockedOn = true;
                            _lockedPlayer = true;
                            _playerRigid = col.GetComponent<Rigidbody2D>();
                            _playerRigid.isKinematic = true;
                            _playerRigid.position = _hurtBox.transform.position;
                        }
                        else
                        {
                            Vector2 direction = (col.transform.position - transform.position).normalized;
                            col.GetComponent<PlayerInterrupt>().Stagger(1,_knockBackVector * direction * 0.5f);
                            _playerStat.DamageCalc(_enemySkills[_attackIndex].damage,_enemySkills[_attackIndex].attribute,false);
                            col.gameObject.GetComponent<DamageEffect>().TriggerEffect((int)_enemySkills[_attackIndex].attribute);
                            _playerContJan = null;
                        }
                        didHit = true;
                    }
                }
            }
        }

        if (didHit)
        {
            // Question mark acts as a null check to avoid invoking an action if not initialized somehow
            CameraFollow.StartShake?.Invoke();
            if (!lockedOn)
            {
                if (_attackWindUp != null)
                {
                    StopCoroutine(_attackWindUp);
                    _attackWindUp = null;
                }
                Recovery();
                _anim.SetTrigger("endAttack");
            }
        }
        else
        {
            if (_attackWindUp != null)
            {
                StopCoroutine(_attackWindUp);
                _attackWindUp = null;
            }
            Recovery();
            _anim.SetTrigger("endAttack");
        }
    }
    void Update()
    {
        // if (_enemyStats.healthRatio <= 0f && _playerContJan != null)
        // {
        //     Recovery();
        // }
        if (_canAttack)
        {
            SelectAttack();
            if (_attackIndex != -1)
            {
                ChargingAttacks(2f,10f);
            }
        }
    }
}