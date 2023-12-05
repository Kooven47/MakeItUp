using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetSpeedrunTimerText : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private GameObject timerObject;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("IsSpeedrunTimerEnabled"))
        {
            timerObject.SetActive(PlayerPrefs.GetInt("IsSpeedrunTimerEnabled") == 1);
        }
        
        timerText.SetText("Time: 00:00.00");    
    }

    // Update is called once per frame
    void Update()
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
