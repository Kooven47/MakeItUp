using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public GameObject gameOverMenu;

    // Start is called before the first frame update
    void Start()
    {
        gameOverMenu.SetActive(false);
    }

    void Update()
    {
        if (PlayerStats.playerIsDead) GameOver();
    }

    public void GameOver()
    {
        gameOverMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void GoToMainMenu()
    {
        PlayerStats.playerIsDead = false;
        gameOverMenu.SetActive(false);
        Time.timeScale = 1;
        SceneManager.LoadScene("Start Screen");
    }

    public void RestartLevel()
    {
        PlayerStats.playerIsDead = false;
        gameOverMenu.SetActive(false);
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void QuitGame()
    {
        PlayerStats.playerIsDead = false;
        Debug.Log("Pressed quit game!");
        Application.Quit();
    }
}