using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour, ISaveGame
{
    bool _readyToLoad = false;
    public void NewGame()
    {
        _readyToLoad = true;
        SaveSystem.instance.SaveGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadGame()
    {
        // TODO: INSERT LOADING FROM SAVE FUNCTIONALITY
        _readyToLoad = true;
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
        if (_readyToLoad)
            SceneManager.LoadScene(data.currentLevel);
    }

    public void LoadInitialData(SaveData data)
    {
        if (_readyToLoad)
            SceneManager.LoadScene(data.currentLevel);
    }

    public void SaveData(ref SaveData data)
    {
        if (data.currentLevel < (SceneManager.GetActiveScene().buildIndex + 2) || _readyToLoad)
        {
            data.currentLevel = SceneManager.GetActiveScene().buildIndex + 2;
            data.numObjectivesCompleted = 0;
        }
    }

    public void SaveInitialData(ref SaveData data)
    {
        if (data.currentLevel < (SceneManager.GetActiveScene().buildIndex + 2) || _readyToLoad)
        {
            data.currentLevel = SceneManager.GetActiveScene().buildIndex + 2;
            data.numObjectivesCompleted = 0;
        }
    }
}
