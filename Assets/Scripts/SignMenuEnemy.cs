using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class SignMenuEnemy : MonoBehaviour
{
    public GameObject signMenu;
    public static bool isMenuActive;

    public SpawnManager spawnManager;
    [SerializeField] private Transform spaghettiEnemyLocations;
    [SerializeField] private Transform dustBunnyEnemyLocations;

    bool shownBefore;

    // Start is called before the first frame update
    void Start()
    {
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
            if(spaghettiEnemyLocations != null)
                spawnManager.SpawnEnemy(spaghettiEnemyLocations, 0);
            if(dustBunnyEnemyLocations != null)
                spawnManager.SpawnEnemy(dustBunnyEnemyLocations, 1);
        }
    }

    public void ShowSign()
    {
        if (!shownBefore)
        { 
            PauseGame();
        }
    }

}