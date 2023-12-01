using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DamageNumberPool : MonoBehaviour
{
    [SerializeField]GameObject _damageNumPrefab;
    [SerializeField]int _damageNumCount = 10;
    [SerializeField]float _positionVariance = 0.5f;
    Queue<DamageNumbers> _damageNumbers = new Queue<DamageNumbers>();

    public static Action<DamageNumbers> returnToPool;
    public static Action<float,int,Vector2, bool> summonDamageNum;
    void Start()
    {
        GameObject temp;
        for (int i = 0; i < _damageNumCount; i++)
        {
            temp = Instantiate(_damageNumPrefab);
            temp.transform.SetParent(transform);
            _damageNumbers.Enqueue(temp.GetComponent<DamageNumbers>());
            temp.GetComponent<DamageNumbers>().Initialize();
            temp.SetActive(false);
        }

        returnToPool = ReturnToPool;
        summonDamageNum = SpawnDamageNumber;
    }

    public void SpawnDamageNumber(float value, int effective ,Vector2 position, bool isCrit)
    {
        DamageNumbers temp = _damageNumbers.Dequeue();
        temp.transform.position = new Vector2(position.x + UnityEngine.Random.Range(-_positionVariance,_positionVariance), position.y + UnityEngine.Random.Range(-_positionVariance,_positionVariance));
        temp.SetValue(value,effective, isCrit);
    }

    public void ReturnToPool(DamageNumbers damage)
    {
        _damageNumbers.Enqueue(damage);
    }
}