using UnityEngine;
using TMPro; // Include TextMeshPro namespace

public class DisplaySettings : MonoBehaviour
{
    public TextMeshProUGUI displayText; // Reference to a TMP Text element

    private bool isSpeedrunTimerEnabled = true;
    private bool isFullscreen = true;
    
    private void Start()
    {
        // Load the fullscreen preference at the start of the game
        if (PlayerPrefs.HasKey("IsFullscreen"))
        {
            isFullscreen = PlayerPrefs.GetInt("IsFullscreen") == 1;
            SetFullscreen();
        }
        
        if (PlayerPrefs.HasKey("IsSpeedrunTimerEnabled"))
        {
            isSpeedrunTimerEnabled = PlayerPrefs.GetInt("IsSpeedrunTimerEnabled") == 1;
        }
        
        UpdateDisplayText();
    }

    public void ToggleFullscreenMode()
    {
        isFullscreen = !Screen.fullScreen;
        SetFullscreen();

        // Save the preference
        PlayerPrefs.SetInt("IsFullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();

        // Update the display text
        UpdateDisplayText();
    }

    private void SetFullscreen()
    {
        Screen.fullScreen = isFullscreen;

        // Optional: Set a specific resolution when not in fullscreen
        if (!isFullscreen)
        {
            Screen.SetResolution(1920, 1080, false);
        }
    }

    public void ToggleSpeedrunTimer()
    {
        isSpeedrunTimerEnabled = !isSpeedrunTimerEnabled;

        // Save the preference
        PlayerPrefs.SetInt("IsSpeedrunTimerEnabled", isSpeedrunTimerEnabled ? 1 : 0);
        PlayerPrefs.Save();

        // Update the display text
        UpdateDisplayText();
    }
    
    private void UpdateDisplayText()
    {
        if (displayText != null)
        {
            string topText = "Current Options:";
            string modeText = isFullscreen ? "Fullscreen" : "Windowed";
            string timerText = isSpeedrunTimerEnabled ? "Timer Enabled" : "Timer Disabled";
            displayText.text = topText + "\n" + modeText + "\n" + timerText;
        }
    }
}
