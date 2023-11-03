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



    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            if (_typeofPowerUp == PowerUpCategory.Heal)
            {
                col.GetComponent<PlayerStats>().RestoreHealth(_effectValue);
                gameObject.SetActive(false);
            }
        }
    }
}