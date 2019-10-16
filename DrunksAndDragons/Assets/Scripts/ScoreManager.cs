﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{

    //public GameObject gameOver;
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

    public GameObject p1_UI;
    public GameObject p2_UI;
    public GameObject p3_UI;
    public GameObject p4_UI;


    public Text gameEndText;


    public bool pointsShown;

    float gameEndTimer = 0;
    float gameEndTime = 6;


    public WaveSpawn waveMaster;
    public int finalWaveNumber;
    private bool gameLost;
    //public GameObject endScreen;

    /// <summary>
    /// Start is called before the first frame update and sets the variables that need setting
    /// </summary>
    public void Start()
    {
        players = new List<PlayerPoints>();
        players.Add(Player1);
        players.Add(Player2);
        pointsShown = false; 
    }
    

    public void checkPlayers()
    {
        players.Clear();
        if (Player1.gameObject.activeInHierarchy) players.Add(Player1);
        else p1_UI.SetActive(false);

        if (Player2.gameObject.activeInHierarchy) players.Add(Player2);
        else p2_UI.SetActive(false);

        if (Player3.gameObject.activeInHierarchy) players.Add(Player3);
        else p3_UI.SetActive(false);

        if (Player4.gameObject.activeInHierarchy) players.Add(Player4);
        else p4_UI.SetActive(false);


    }

    /// <summary>
    /// Update is called once per frame and updates the timer if needed
    /// </summary>
    void Update()
    {
        checkPlayers();

        //Shows and updates Player 1 score
        p1_Score.text = "Player 1:    " + Player1.getKills() + " Kills | " + Player1.GetPoints() + " Gold";
        //Shows and updates Player 2 score
        p2_Score.text = "Player 2:    " + Player2.getKills() + " Kills | " + Player2.GetPoints() + " Gold";
        //Shows and updates Player 3 score
        p3_Score.text = "Player 3:    " + Player3.getKills() + " Kills | " + Player3.GetPoints() + " Gold";
        //Shows and updates Player 4 score
        p4_Score.text = "Player 4:    " + Player4.getKills() + " Kills | " + Player4.GetPoints() + " Gold";
        //time ends new wave starts
        
        bool playersAlive = false;
        foreach (PlayerPoints child in players)
            if (child.GetComponent<PlayerDamageHandler>().lives > 0)
                playersAlive = true;
        gameLost = !playersAlive;
        if (waveMaster.waveCount == finalWaveNumber + 1 || gameLost)
        {
            if(gameLost)
            {
                gameEndText.text = "Game Over";
            }
            else
            {
                gameEndText.text = "Level Complete";
            }

            //gameover
            //gameOver.SetActive(true);
            results.SetActive(true);
            if (!pointsShown)
            {
                GameOverPoints();
            }
            //else if(gameEndTimer > 0)
            //{
            //    gameEndTimer -= Time.unscaledDeltaTime;
            //    
            //}
            if (XboxCtrlrInput.XCI.GetButtonDown(XboxCtrlrInput.XboxButton.A, XboxCtrlrInput.XboxController.All) || Input.GetKey(KeyCode.Backspace))
            {
                Time.timeScale = 1;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }


    /// <summary>
    /// Tallies up all player scores and prints them in order of rank
    /// </summary>
    public void GameOverPoints()
    {

        int[] scores = new int[4];
        GameObject[] ranks = new GameObject[4];


        foreach(PlayerPoints child in players)
        {
            if(scores.Length == 0)
            {
                scores[0] = child.getKills();
                ranks[0] = child.gameObject;
                continue;
            }
            for(int i = 0; i < scores.Length; i++)
            {
                if(child.getKills() > scores[i])
                {
                    for(int j = 3; j > -1; j--)
                    {
                        if (j == i)
                            break;
                        scores[j] = scores[j - 1];
                        ranks[j] = ranks[j - 1];

                    }
                    scores[i] = child.getKills();
                    ranks[i] = child.gameObject;
                    break;
                }
                if (!ranks[i])
                {
                    scores[i] = child.getKills();
                    ranks[i] = child.gameObject;
                    break;
                }
            }

        }

        string temp = "Results" + "\n" + "\n";

        for(int i = 0; i < players.Count; i++)
        {
            temp += i+1 + ". " + ranks[i].name + ":       " + scores[i] + "\n" + "\n";
        }
        //gameOver.GetComponent<Text>().text = temp;
        pointsShown = true;

        Time.timeScale = 0;
        gameEndTimer = gameEndTime;
    }

    

}
