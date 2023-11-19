using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WaterCoolerCore : EnemyCore
{
    [SerializeField] private GameObject _waterBeam, _waterBeamAim;
    [SerializeField] private float _beamSpeed = 5f;

    public bool _beamActive
    {
        get{return (_waterBeam != null ? _waterBeam.activeSelf : false);}
    }
    protected override void Start()
    {
        base.Start();
        _waterBeamAim.SetActive(false);
        _waterBeam.SetActive(false);
    }

    protected override IEnumerator ChargeTimers(Vector2 chargeTime, bool isWindUp)
    {
        
        if (isWindUp)
        {
            float aimTime = chargeTime.x * 0.8f;
            float finalTime = 1 - aimTime;
            _waterBeamAim.SetActive(true);
            while (aimTime > 0f)
            {
                yield return new WaitForSeconds(Time.deltaTime);
                aimTime -= Time.deltaTime;
                _waterBeamAim.transform.rotation = Quaternion.Slerp(_waterBeamAim.transform.rotation,AnglefromVector(_target.position - _waterBeamAim.transform.position),Time.deltaTime * _beamSpeed);
            }
            _waterBeamAim.SetActive(false);
            _waterBeam.SetActive(true);
            _waterBeam.transform.rotation = _waterBeamAim.transform.rotation;
            while(finalTime > 0f)
            {
                yield return new WaitForSeconds(Time.deltaTime);
                finalTime -= Time.deltaTime;
            }
            
            _anim.SetTrigger("release");
            _attackWindUp = StartCoroutine(ChargeTimers(chargeTime,false));
        }
        else
        {
            yield return new WaitForSeconds(chargeTime.y);
            _waterBeam.SetActive(false);
            _anim.SetTrigger("endAttack");
            _attackWindUp = null;
            Recovery();
        }
    }

    protected override void SelectAttack()
    {
        //_waterBeam.transform.eulerAngles = new Vector3(0f,0f,AnglefromVector(Direction()));
        //_waterBeam.transform.rotation = Quaternion.Slerp(_waterBeam.transform.rotation,AnglefromVector(_target.position - _waterBeam.transform.position),Time.deltaTime * _beamSpeed);
        if (InDistance(_projectileRange))
        {
            _canAttack = false;
            _attackIndex = 0;
            _knockBackVector = Vector2.zero;
        }
    }

    protected void BeamStrike()
    {
        Debug.Log("Biden blast!");
        List<Collider2D> targets = new List<Collider2D>();
        Physics2D.OverlapCollider(_hurtBox,_hurtLayers,targets);
        bool didHit = false;

        PlayerStats _playerStat;

        if (targets.Count != 0)
        {
            foreach(Collider2D col in targets)
            {
                if (col.CompareTag("Player"))
                {
                    Debug.Log("Hit the janitor with biden blast!");
                    _playerStat = col.GetComponent<PlayerStats>();
                    if (!_playerStat.iFrame)
                    {
                        Vector2 direction = (col.transform.position - transform.position).normalized;
                        col.GetComponent<PlayerInterrupt>().Stagger(1,_knockBackVector * direction * 0.5f);
                        _playerStat.DamageCalc(_enemySkills[_attackIndex].damage,_enemySkills[_attackIndex].attribute,false);
                        col.gameObject.GetComponent<DamageEffect>().TriggerEffect((int)_enemySkills[_attackIndex].attribute);
                        didHit = true;
                    }
                    else
                    {
                        Debug.Log("Under Iframes");
                    }
                }
            }
        }

        if (didHit)
        {
            // Question mark acts as a null check to avoid invoking an action if not initialized somehow
            CameraFollow.StartShake?.Invoke();
        }
    }

    void FixedUpdate()
    {
        if (_beamActive)
        {
            BeamStrike();
        }
    }

    void Update()
    {
        if (_canAttack)
        {
            SelectAttack();
            if (_attackIndex != -1)
            {
                ChargingAttacks(4f,3f);
            }
        }
    }
}