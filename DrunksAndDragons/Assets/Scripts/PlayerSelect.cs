using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSelect : MonoBehaviour
{
    public GameObject Player1;
    public GameObject Player2;
    public GameObject Player3;
    public GameObject Player4;

    public static bool player1Active = true;
    public static bool player2Active = true;
    public static bool player3Active = true;
    public static bool player4Active = true;
    
    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            SelectPlayers();
            ActivatePlayers();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                //make more efficient later
                //loads next scene after character select
                SceneManager.LoadScene(1);
            }
        }
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            CheckActivePlayer();
        }
    }

    /// <summary>
    /// uses the keys 1-4 on the top of the keyboard to activate their players
    /// </summary>
    private void SelectPlayers()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Player1.SetActive(!Player1.activeInHierarchy);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Player2.SetActive(!Player2.activeInHierarchy);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Player3.SetActive(!Player3.activeInHierarchy);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Player4.SetActive(!Player4.activeInHierarchy);
        }
    }

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