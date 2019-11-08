using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XboxCtrlrInput;

public class testPlayerSelection : MonoBehaviour
{
    //private XboxController Controller1 = XboxController.First;
    //private XboxController Controller2 = XboxController.Second;
    //private XboxController Controller3 = XboxController.Third;
    //private XboxController Controller4 = XboxController.Fourth;
    //public GameObject Player1;
    //public GameObject Player2;
    //public GameObject Player3;
    //public GameObject Player4;

    public GameObject[] players;

    public string playerSelectScene;
    public string gameScene;

    private static bool player1Active = true;
    private static bool player2Active = true;
    private static bool player3Active = true;
    private static bool player4Active = true;

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        if (SceneManager.GetActiveScene().name == playerSelectScene)
        {
            SelectPlayers();
            ActivatePlayers();

            if (XCI.GetButton(XboxButton.Back, XboxController.First))
            {
                //make more efficient later
                //loads next scene after character select
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
        if (SceneManager.GetActiveScene().name == gameScene)
        {
            CheckActivePlayer();
        }
    }

    /// <summary>
    /// uses the keys 1-4 on the top of the keyboard to activate their players
    /// </summary>
    private void SelectPlayers()
    {
        if (XCI.GetButtonDown(XboxButton.Start, XboxController.First))
        {
            for(int i = 0; i < players.Length; i++)
            {
                if(!players[i].activeInHierarchy)
                {
                    players[i].SetActive(!players[0].activeInHierarchy);
                    return;
                }
            }
        }
        if (XCI.GetButtonDown(XboxButton.Start, XboxController.Second))
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (!players[i].activeInHierarchy)
                {
                    players[i].SetActive(!players[0].activeInHierarchy);
                    return;
                }
            }
        }
        if (XCI.GetButtonDown(XboxButton.Start, XboxController.Third))
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (!players[i].activeInHierarchy)
                {
                    players[i].SetActive(!players[0].activeInHierarchy);
                    return;
                }
            }
        }
        if (XCI.GetButtonDown(XboxButton.Start, XboxController.Fourth))
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (!players[i].activeInHierarchy)
                {
                    players[i].SetActive(!players[0].activeInHierarchy);
                    return;
                }
            }
        }
    }

    /// <summary>
    /// checks to see if the bool is active, if it is it will activate the corresponding players
    /// </summary>
    private void CheckActivePlayer()
    {
        if (player1Active)
        {
            players[0].SetActive(true);
        }
        else
        {
            players[0].SetActive(false);
        }

        if (player2Active)
        {
            players[1].SetActive(true);
        }
        else
        {
            players[1].SetActive(false);
        }

        if (player3Active)
        {
            players[2].SetActive(true);
        }
        else
        {
            players[2].SetActive(false);
        }

        if (player4Active)
        {
            players[3].SetActive(true);
        }
        else
        {
            players[3].SetActive(false);
        }
    }

    /// <summary>
    /// Checks which players are active so it can set the bool between scenes to activate them in main scene
    /// </summary>
    private void ActivatePlayers()
    {

        if (players[0].activeInHierarchy)
        {
            player1Active = true;
        }
        else
        {
            player1Active = false;
        }
        if (players[1].activeInHierarchy)
        {
            player2Active = true;
        }
        else
        {
            player2Active = false;
        }
        if (players[2].activeInHierarchy)
        {
            player3Active = true;
        }
        else
        {
            player3Active = false;
        }
        if (players[3].activeInHierarchy)
        {
            player4Active = true;
        }
        else
        {
            player4Active = false;
        }
    }
}