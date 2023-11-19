using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WaterCoolerCore : EnemyCore
{
    [SerializeField] private GameObject _waterBeam;
    [SerializeField] private float _beamSpeed = 5f;
    protected override void Start()
    {
        base.Start();
    }

    protected override void SelectAttack()
    {
        //_waterBeam.transform.eulerAngles = new Vector3(0f,0f,AnglefromVector(Direction()));
        _waterBeam.transform.rotation = Quaternion.Slerp(_waterBeam.transform.rotation,AnglefromVector(_target.position - transform.position),Time.deltaTime * _beamSpeed);
    }

    void Update()
    {
        if (_canAttack)
        {
            SelectAttack();
            if (_attackIndex != -1)
            {
                ReadyAttack();
            }
        }
    }
}