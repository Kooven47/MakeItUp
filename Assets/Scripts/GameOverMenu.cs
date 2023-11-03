using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public GameObject gameOverMenu;
    public static Action gameOver;
    public static event Action cleanUp;
    public static bool isMenuActive;
    // Start is called before the first frame update
    void Start()
    {
        isMenuActive = false;
        gameOverMenu.SetActive(false);
        gameOver = GameOver;
    }

    // void Update()
    // {
    //     if (PlayerStats.playerIsDead) GameOver();
    // }

    public void GameOver()
    {
        gameOverMenu.SetActive(true);
        isMenuActive = true;
        Time.timeScale = 0;
    }

    public void GoToMainMenu()
    {
        PlayerStats.playerIsDead = false;
        gameOverMenu.SetActive(false);
        Time.timeScale = 1;
        cleanUp?.Invoke();
        isMenuActive = false;
        SceneManager.LoadScene("Start Screen");
    }

    public void RestartLevel()
    {
        gameOverMenu.SetActive(false);
        Time.timeScale = 1;
        cleanUp?.Invoke();
        isMenuActive = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void QuitGame()
    {
        PlayerStats.playerIsDead = false;
        Debug.Log("Pressed quit game!");
        isMenuActive = false;
        Application.Quit();
    }
}