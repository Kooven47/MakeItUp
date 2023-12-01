using UnityEngine;
using TMPro; // Include TextMeshPro namespace

public class ToggleFullscreen : MonoBehaviour
{
    public TextMeshProUGUI modeDisplayText; // Reference to a TMP Text element

    private void Start()
    {
        // Load the fullscreen preference at the start of the game
        if (PlayerPrefs.HasKey("IsFullscreen"))
        {
            bool isFullscreen = PlayerPrefs.GetInt("IsFullscreen") == 1;
            SetFullscreen(isFullscreen);
            UpdateModeDisplayText(Screen.fullScreen);
        }
        else
        {
            // Update the display text based on the current mode
            UpdateModeDisplayText(Screen.fullScreen);
        }
    }

    public void ToggleFullscreenMode()
    {
        bool isFullscreen = !Screen.fullScreen;
        SetFullscreen(isFullscreen);

        // Save the preference
        PlayerPrefs.SetInt("IsFullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();

        // Update the display text
        UpdateModeDisplayText(isFullscreen);
    }

    private void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;

        // Optional: Set a specific resolution when not in fullscreen
        if (!isFullscreen)
        {
            Screen.SetResolution(1920, 1080, false);
        }
    }

    private void UpdateModeDisplayText(bool isFullscreen)
    {
        if (modeDisplayText != null)
        {
            modeDisplayText.text = isFullscreen ? "Current Option: Fullscreen" : "Current Option: Windowed";
        }
    }
}
