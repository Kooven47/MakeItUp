using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{
    public void NewGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadGame()
    {
        // TODO: INSERT LOADING FROM SAVE FUNCTIONALITY
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
}
