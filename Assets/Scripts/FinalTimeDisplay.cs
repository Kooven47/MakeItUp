using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FinalTimeDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;

    // Start is called before the first frame update
    void Start()
    {
        TimeSpan currentTime = GlobalSpeedrunTimer.GetTime();
        string formattedTime = FormatTime(currentTime);
        timerText.SetText($"Time: {formattedTime}");
    }

    // Format the TimeSpan object to display hours, minutes, and seconds
    private string FormatTime(TimeSpan timeSpan)
    {
        int minutes = timeSpan.Minutes;
        int seconds = timeSpan.Seconds;
        int milliseconds = timeSpan.Milliseconds;

        return $"{minutes:00}:{seconds:00}.{milliseconds:000}";
    }
}
