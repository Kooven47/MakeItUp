using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class SignMenuEnemy : MonoBehaviour
{
    public GameObject signMenu;
    public static bool isMenuActive;

    public SpawnManager spawnManager;
    public Transform enemyLocations;

    bool shownBefore;

    // Start is called before the first frame update
    void Start()
    {
        shownBefore = false;
        isMenuActive = false;
        signMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PauseGame()
    {
        isMenuActive = true;
        signMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        isMenuActive = false;
        signMenu.SetActive(false);
        shownBefore = true;
        if (!PauseMenu.isPaused)
        {
            Time.timeScale = 1;
            spawnManager.SpawnEnemy(enemyLocations);
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