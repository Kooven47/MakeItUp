using UnityEngine;
using System;
public class PlayerStatus : MonoBehaviour
{
    public static PlayerStatus Instance { get; private set; }
    GameObject _player;
    PlayerStats _playerStats;

    public float playerHPRatio
    {
        get{
            if (_playerStats != null)
                return _playerStats.healthRatio;
            else
                return -1f;
        }
    }
    private void Awake() 
    { 
        // If there is an instance, and it's not me, delete myself.
        
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }

    private void Start()
    {
        _player = GameObject.Find("Janitor");
        _playerStats = _player.GetComponent<PlayerStats>();
    }
}