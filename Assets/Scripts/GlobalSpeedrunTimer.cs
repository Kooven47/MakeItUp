using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public static class GlobalSpeedrunTimer
{
    private static Stopwatch stopwatch;
    private static TimeSpan currentTime;
    private static bool isTimerRunning = false;
    
    public static void StartTimer()
    {
        if (stopwatch == null)
        {
            stopwatch = new Stopwatch();
        }
        
        stopwatch.Start();
        isTimerRunning = true;
    }
    
    public static void StopTimer()
    {
        if (stopwatch != null)
        {
            stopwatch.Stop();
            isTimerRunning = false;
        }
    }
    
    public static void ResetTimer()
    {
        if (stopwatch != null)
        {
            stopwatch.Reset();
            isTimerRunning = false;
        }
    }
    
    public static TimeSpan GetTime()
    {
        if (stopwatch != null)
        {
            currentTime = stopwatch.Elapsed;
            return currentTime;
        }
        
        return TimeSpan.Zero;
    }
    
    public static bool IsTimerRunning()
    {
        return isTimerRunning;
    }
}
