using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text timer;
    public float timeLength;
    public float maxTime;
    public int wave;
    public int maxWaves;
    public GameObject gameOver;

    /// <summary>
    /// Start is called before the first frame update and sets the variables that need setting
    /// </summary>
    void Start()
    {
        timeLength = maxTime;
        wave = 1; 
    }

    // 
    /// <summary>
    /// Update is called once per frame and updates the timer if needed
    /// </summary>
    void Update()
    {
        //time ends new wave starts
        if(timeLength > 0)
        {
            TimerUpdate();

        }
        else
        {
            if (wave < maxWaves)
            {
                //time is over which means wave over
                wave++;
                timeLength = maxTime;
            }
            else
            {
                //gameover
                gameOver.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Timer update manages the timer and will change it accordingly
    /// </summary>
    void TimerUpdate()
    {
        timeLength -= Time.deltaTime;
        int IntTime = Mathf.RoundToInt(timeLength);
        timer.text = IntTime.ToString(); 
    }
}
