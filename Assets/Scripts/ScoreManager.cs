using System;
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

    public GameObject Player1;
    public GameObject Player2;
    public GameObject Player3;
    public GameObject Player4;

    public Text p1_Score;
    public Text p2_Score;
    public Text p3_Score;
    public Text p4_Score;


    public bool pointsShown;

    /// <summary>
    /// Start is called before the first frame update and sets the variables that need setting
    /// </summary>
    public void Start()
    {
        gameOver.SetActive(false);
        pointsShown = false; 
        timeLength = maxTime;
        wave = 1; 
    }

    /// <summary>
    /// Update is called once per frame and updates the timer if needed
    /// </summary>
    void Update()
    {
        //Shows and updates Player 1 score
        int P1Points = PlayerPoints(Player1);
        p1_Score.text = "Score: " + P1Points;
        //Shows and updates Player 2 score
        int P2Points = PlayerPoints(Player2);
        p2_Score.text = "Score: " + P2Points;
        //Shows and updates Player 3 score
        int P3Points = PlayerPoints(Player3);
        p3_Score.text = "Score: " + P3Points;
        //Shows and updates Player 4 score
        int P4Points = PlayerPoints(Player4);
        p4_Score.text = "Score: " + P4Points;
        //time ends new wave starts
        if (timeLength > 0.99)
        {
            TimerUpdate();
        }
        else
        {
            //gameover
            gameOver.SetActive(true);
            if (!pointsShown)
            {
                timeLength = 0.99f;
                GameOverPoints();
            }
        }
    }

    /// <summary>
    /// Timer update manages the timer and will change it accordingly
    /// </summary>
    void TimerUpdate()
    {
        timeLength -= Time.deltaTime;
        string minutes = ((int)timeLength / 60).ToString("00");
        string seconds = Mathf.Floor(timeLength % 60).ToString("00");
        timer.text = minutes + ":" + seconds;
    }

    private int PlayerPoints(GameObject player)
    {
        return player.GetComponent<PlayerPoints>().GetPoints();
    }

    /// <summary>
    /// gets all the players scores and 
    /// </summary>
    public void GameOverPoints()
    {
        //player1.getpoints
        int P1Points = PlayerPoints(Player1);
        //player2.getpoints
        int P2Points = PlayerPoints(Player2);
        //player4.getpoints
        int P3Points = PlayerPoints(Player3);
        //player3.getpoints
        int P4Points = PlayerPoints(Player4);
        //compare.player.points
        int[] points = { P1Points, P2Points, P3Points, P4Points };
        //sort.players
        Array.Sort(points);
        Array.Reverse(points);
        foreach(int score in points)
        {
            print(score);
        }
        //winner is the player with most points
        //print(points[0]);
        pointsShown = true;
    }


}
