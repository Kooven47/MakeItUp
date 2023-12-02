using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public GameObject gameOverMenu;
    public static Action gameOver;
    public static event Action cleanUp;
    public static bool isMenuActive;
    
    public GameObject confirmationMenu;
    public static bool isConfirmationMenuActive;
    
    private PlayerControllerJanitor _playerControllerJanitor;


    // Start is called before the first frame update
    void Start()
    {
        _playerControllerJanitor = GameObject.FindWithTag("Player").GetComponent<PlayerControllerJanitor>();
        isMenuActive = false;
        gameOverMenu.SetActive(false);
        gameOver = GameOver;
        confirmationMenu.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isConfirmationMenuActive && PlayerStats.playerIsDead)
        {
            confirmationMenu.SetActive(false);
            gameOverMenu.SetActive(true);
            isConfirmationMenuActive = false;
        }
    }

    public void GameOver()
    {
        const int DEAD = 12;
        _playerControllerJanitor.PlaySoundEffect(DEAD);
        PlayerStats.playerIsDead = true;
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
        SceneManager.LoadScene("StartScreen");
    }
    
    public void RestartLevel()
    {
        if (!isConfirmationMenuActive)
        {
            gameOverMenu.SetActive(false);
            confirmationMenu.SetActive(true);
            isConfirmationMenuActive = true;
        }
    }
    
    public void ConfirmRestart()
    {
        CheckpointManager.resetCheckPoint?.Invoke();
        RestartFromCheckpoint();
    }

    public void CancelRestart()
    {
        confirmationMenu.SetActive(false);
        gameOverMenu.SetActive(true);
        isConfirmationMenuActive = false;
    }

    public void RestartFromCheckpoint()
    {
        PlayerStats.playerIsDead = false;
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