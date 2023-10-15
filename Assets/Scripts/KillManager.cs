using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KillManager : MonoBehaviour
{
    private static int killCount;
    
    void Start()
    {
        // Set killCount to 0 at the start
        killCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnEnable()
    {
        EnemyStats.OnDeath += KillIncrement;
    }

    private void OnDisable()
    {
        EnemyStats.OnDeath -= KillIncrement;
    }

    private void KillIncrement()
    {
        killCount++;
    }

    public void KillDecrement()
    {
        killCount--;
    }
    public void ClearAllKills()
    {
        killCount = 0;
    }
    public int GetKillCount()
    {
        return killCount;
    }

}
