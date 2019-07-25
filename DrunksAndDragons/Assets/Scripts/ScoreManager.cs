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

    // Start is called before the first frame update
    void Start()
    {
        timeLength = maxTime;
        wave = 1; 
    }

    // Update is called once per frame
    void Update()
    {
        if(timeLength > 0)
        {
            TimerUpdate();

        }
        else
        {
            if (wave < maxWaves)
            {
                wave++;
                timeLength = maxTime;
            }
            else
            {
                //gameover
                gameOver.SetActive(true);
            }
            //time is over which means round over
        }
    }

    void TimerUpdate()
    {
        timeLength -= Time.deltaTime;
        int IntTime = Mathf.RoundToInt(timeLength);
        timer.text = IntTime.ToString(); 
    }
}
