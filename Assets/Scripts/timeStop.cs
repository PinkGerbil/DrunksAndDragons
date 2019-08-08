using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timeStop : MonoBehaviour
{
    [SerializeField]
    [Range(0, 5)]
    float timeStopDuration = 0.25f;
    float timeStopCountdown = 0.0f;

    bool timeStopped = false;

    // Update is called once per frame
    void Update()
    {
        if (timeStopCountdown > 0)
            timeStopCountdown -= Time.unscaledDeltaTime;
        else if (timeStopped)
        {
            Time.timeScale = 1.0f;
            timeStopped = false;
        }
    }

    /// <summary>
    /// Sets timescale to 0 for a set amount of unscaled time)
    /// </summary>
    public void enableTimeStop()
    { 
        timeStopCountdown = timeStopDuration;
        Time.timeScale = 0.0f;
        timeStopped = true;
    }
}
