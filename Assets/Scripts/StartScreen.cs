using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour, ISaveGame
{
    bool _readyToLoad = false;
    [SerializeField] private GameObject _firstSelections;
    [SerializeField] private GameObject _Settings;
    [SerializeField] private GameObject _SoundSettings;
    [SerializeField] private GameObject _DisplaySettings;

    void Start()
    {
        _firstSelections.SetActive(true);
        _Settings.SetActive(false);
        _SoundSettings.SetActive(false);
        _DisplaySettings.SetActive(false);
    }

    public void NewGame()
    {
        _readyToLoad = true;
        SaveSystem.instance.SaveGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadGame()
    {
        _readyToLoad = true;
        SaveSystem.instance.LoadGame();
        Debug.Log("Pressed load game!");
    }

    public void Options()
    {
        _firstSelections.SetActive(false);
        _Settings.SetActive(true);
        Debug.Log("Pressed options!");
    }
    public void OptionsBackButton()
    {
        _firstSelections.SetActive(true);
        _Settings.SetActive(false);
        Debug.Log("Pressed options back button!");
    }
    public void SoundOptions()
    {
        _Settings.SetActive(false);
        _SoundSettings.SetActive(true);
        Debug.Log("Pressed sound options!");
    }
    public void SoundOptionsBackButton()
    {
        _Settings.SetActive(true);
        _SoundSettings.SetActive(false);
        Debug.Log("Pressed sound options back button!");
    }
    public void DisplayOption()
    {
        _Settings.SetActive(false);
        _DisplaySettings.SetActive(true);
        Debug.Log("Pressed display options button!");
    }
    public void DisplayOptionBackButton()
    {
        _Settings.SetActive(true);
        _DisplaySettings.SetActive(false);
        Debug.Log("Pressed display options back button!");
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
