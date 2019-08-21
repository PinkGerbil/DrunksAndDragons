using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public Text timer;
    public float timeLength;
    public float maxTime;
    public int wave;
    public int maxWaves;
    public GameObject gameOver;
    public GameObject results;

    List<PlayerPoints> players;
    public PlayerPoints Player1;
    public PlayerPoints Player2;
    public PlayerPoints Player3;
    public PlayerPoints Player4;

    public Text p1_Score;
    public Text p2_Score;
    public Text p3_Score;
    public Text p4_Score;


    public bool pointsShown;

    float gameEndTimer = 0;
    float gameEndTime = 5;

    /// <summary>
    /// Start is called before the first frame update and sets the variables that need setting
    /// </summary>
    public void Start()
    {
        players = new List<PlayerPoints>();
        players.Add(Player1);
        players.Add(Player2);
        players.Add(Player3);
        players.Add(Player4);
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
        p1_Score.text = "Score: " + Player1.GetPoints();
        //Shows and updates Player 2 score
        p2_Score.text = "Score: " + Player2.GetPoints();
        //Shows and updates Player 3 score
        p3_Score.text = "Score: " + Player3.GetPoints();
        //Shows and updates Player 4 score
        p4_Score.text = "Score: " + Player4.GetPoints();
        //time ends new wave starts
        if (timeLength > 0)
        {
            TimerUpdate();

        }
        else
        {


            //gameover
            gameOver.SetActive(true);
            results.SetActive(true);
            if (!pointsShown)
            {
                GameOverPoints();
            }
            else if(gameEndTimer > 0)
            {
                gameEndTimer -= Time.unscaledDeltaTime;
                
            }
            else
            {
                Time.timeScale = 1;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
        string minutes = ((int)IntTime / 60).ToString("00");
        string seconds = Mathf.Floor(IntTime % 60).ToString("00");

        timer.text = minutes + ":" + seconds; 
    }

    /// <summary>
    /// Tallies up all player scores and prints them in order of rank
    /// </summary>
    public void GameOverPoints()
    {
        int bestScore = -1;

        int[] scores = new int[4];
        GameObject[] ranks = new GameObject[4];


        foreach(PlayerPoints child in players)
        {
            if(scores.Length == 0)
            {
                scores[0] = child.GetPoints();
                ranks[0] = child.gameObject;
                continue;
            }
            for(int i = 0; i < scores.Length; i++)
            {
                if(child.GetPoints() > scores[i])
                {
                    for(int j = 3; j > -1; j--)
                    {
                        if (j == i)
                            break;
                        scores[j] = scores[j - 1];
                        ranks[j] = ranks[j - 1];

                    }
                    scores[i] = child.GetPoints();
                    ranks[i] = child.gameObject;
                    break;
                }
                if (!ranks[i])
                {
                    scores[i] = child.GetPoints();
                    ranks[i] = child.gameObject;
                    break;
                }
            }

        }

        string temp = "Results" + "\n" + "\n";

        for(int i = 0; i < scores.Length; i++)
        {
            temp += i+1 + ". " + ranks[i].name + ":       " + scores[i] + "\n" + "\n";
        }
        gameOver.GetComponent<Text>().text = temp;
        pointsShown = true;

        Time.timeScale = 0;
        gameEndTimer = gameEndTime;
    }

    

}
