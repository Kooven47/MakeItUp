using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Powerup : MonoBehaviour
{
    public enum PowerUpCategory{Money,Heal,MaxHPUp};
    [SerializeField] private PowerUpCategory _typeofPowerUp;
    [SerializeField] private float _effectValue = 0.0f;
    [SerializeField] private Sprite _icon;
    [SerializeField] private float _existenceTime = 15f;

    private Coroutine _existenceTimer;



    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            if (_typeofPowerUp == PowerUpCategory.Heal)
            {
                col.GetComponent<PlayerStats>().RestoreHealth(_effectValue);
                if (_existenceTimer == null)
                    gameObject.SetActive(false);
                else
                {
                    ReturnPoweruptoPool();
                }
            }
        }
    }

    void ReturnPoweruptoPool()
    {
        PowerupManager.returnToHealingPool?.Invoke(gameObject);
        _existenceTimer = null;
    }

    private IEnumerator ExistenceTimer()
    {
        yield return new WaitForSeconds(_existenceTime);
        ReturnPoweruptoPool();
    }

    public void StartExistenceTimer()
    {
        if (_existenceTimer == null)
        {
            _existenceTimer = StartCoroutine(ExistenceTimer());
        }
    }
}