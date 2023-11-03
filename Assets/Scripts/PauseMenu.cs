using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public static bool isPaused;
    public static event Action cleanUp;
    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        if (!SignMenuCollision.isMenuActive && !SignMenuCollisionWithEnemy.isMenuActive && !SignMenu.isMenuActive && !SignMenuEnemy.isMenuActive && !GameOverMenu.isMenuActive)
            Time.timeScale = 1;
        isPaused = false;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1;
        cleanUp?.Invoke();
        SceneManager.LoadScene("Start Screen");
        isPaused = false;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1;
        isPaused = false;
        cleanUp?.Invoke();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}