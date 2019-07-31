using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XboxCtrlrInput;

public class PlayerSelect : MonoBehaviour
{
    public XboxController Controller1;
    public XboxController Controller2;
    public XboxController Controller3;
    public XboxController Controller4;
    public GameObject Player1;
    public GameObject Player2;
    public GameObject Player3;
    public GameObject Player4;

    public string playerSelectScene;
    public string gameScene;

    public static bool player1Active = true;
    public static bool player2Active = true;
    public static bool player3Active = true;
    public static bool player4Active = true;
    
    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        if(SceneManager.GetActiveScene().name == playerSelectScene)
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
        if (XCI.GetButton(XboxButton.Start, XboxController.First))
        {
            Player1.SetActive(!Player1.activeInHierarchy);
        }
        if (XCI.GetButton(XboxButton.Start, XboxController.Second))
        {
            Player2.SetActive(!Player2.activeInHierarchy);
        }
        if (XCI.GetButton(XboxButton.Start, XboxController.Third))
        {
            Player3.SetActive(!Player3.activeInHierarchy);
        }
        if (XCI.GetButton(XboxButton.Start, XboxController.Fourth))
        {
            Player4.SetActive(!Player4.activeInHierarchy);
        }
    }

    /// <summary>
    /// checks to see if the bool is active, if it is it will activate the corresponding players
    /// </summary>
    private void CheckActivePlayer()
    {
        if (player1Active)
        {
            Player1.SetActive(true);
        }
        else
        {
            Player1.SetActive(false);
        }

        if (player2Active)
        {
            Player2.SetActive(true);
        }
        else
        {
            Player2.SetActive(false);
        }

        if (player3Active)
        {
            Player3.SetActive(true);
        }
        else
        {
            Player3.SetActive(false);
        }

        if (player4Active)
        {
            Player4.SetActive(true);
        }
        else
        {
            Player4.SetActive(false);
        }
    }
    
    /// <summary>
    /// Checks which players are active so it can set the bool between scenes to activate them in main scene
    /// </summary>
    private void ActivatePlayers()
    {
        if (Player1.activeInHierarchy)
        {
            player1Active = true;
        }
        else
        {
            player1Active = false;
        }
        if (Player2.activeInHierarchy)
        {
            player2Active = true;
        }
        else
        {
            player2Active = false;
        }
        if (Player3.activeInHierarchy)
        {
            player3Active = true;
        }
        else
        {
            player3Active = false;
        }
        if (Player4.activeInHierarchy)
        {
            player4Active = true;
        }
        else
        {
            player4Active = false;
        }
    }
}