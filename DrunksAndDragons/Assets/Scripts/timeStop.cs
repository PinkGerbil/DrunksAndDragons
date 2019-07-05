using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timeStop : MonoBehaviour
{
    [SerializeField]
    [Range(0, 5)]
    float timeStopDuration = 0.25f;
    float timeStopCountdown = 0.0f;



    // Update is called once per frame
    void Update()
    {
        if (timeStopCountdown > 0)
            timeStopCountdown -= Time.unscaledDeltaTime;
        else
            Time.timeScale = 1.0f;
    }

    public void enableTimeStop()
    { 
        timeStopCountdown = timeStopDuration;
        Time.timeScale = 0.0f;
    }
}
