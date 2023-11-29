using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour,ISaveGame
{
    public void NewGame()
    {
        SaveSystem.instance.SaveGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadGame()
    {
        // TODO: INSERT LOADING FROM SAVE FUNCTIONALITY
        SaveSystem.instance.LoadGame();
        Debug.Log("Pressed load game!");
    }

    public void Options()
    {
        // TODO: ADD VOLUME CONTROL, CHANGING CONTROL KEYBINDINGS, ETC.
        Debug.Log("Pressed options!");
    }

    public void QuitGame()
    {
        Debug.Log("Pressed quit game!");
        Application.Quit();
    }

    public void LoadSaveData(SaveData data)
    {
        SceneManager.LoadScene(data.currentLevel);
    }

    public void LoadInitialData(SaveData data)
    {
        SceneManager.LoadScene(data.currentLevel);
    }

    public void SaveData(ref SaveData data)
    {
        data.currentLevel = SceneManager.GetActiveScene().buildIndex + 2;
    }

    public void SaveInitialData(ref SaveData data)
    {
        data.currentLevel = SceneManager.GetActiveScene().buildIndex + 2;
    }
}
