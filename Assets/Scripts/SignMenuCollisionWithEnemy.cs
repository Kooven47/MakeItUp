using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Events;
using System;

public class SignMenuCollisionWithEnemy : EnclosureCollision
{
    public GameObject signMenu;
    public static bool isMenuActive;
    public Transform enemyLocations;

    bool shownBefore;
    
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        shownBefore = false;
        isMenuActive = false;
        signMenu.SetActive(false);
    }

    public void PauseGame()
    {
        GlobalSpeedrunTimer.StopTimer();
        isMenuActive = true;
        signMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {   
        GlobalSpeedrunTimer.StartTimer();
        isMenuActive = false;
        signMenu.SetActive(false);
        shownBefore = true;
        if (!PauseMenu.isPaused)
        {
            Time.timeScale = 1;
            spawnManager.SpawnEnemy(enemyLocations, 0);
            ObjectiveManagerLevel1.OnUpdateObjective();
        }

        // SetCheckPoint();
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // When colliding pause the game
            if (!shownBefore)
                PauseGame();
        }
    }
}