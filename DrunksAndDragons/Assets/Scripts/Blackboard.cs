using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blackboard : MonoBehaviour
{
    
    [SerializeField] public GameObject pauseUI;
    [SerializeField] public GameObject settingsUI;

    [HideInInspector]
    public List<GameObject> players = new List<GameObject>();

    bool paused = false;
  

    /// <summary>
    /// Adds a player gameobject to the list of player
    /// </summary>
    /// <param name="player">Game Object being added to list</param>
    public void addPlayer(GameObject player)
    {
        players.Add(player);
    }   

    /// <summary>
    /// Sets the game pause state to the one specified
    /// </summary>
    /// <param name="toggle">Pause state</param>
    public void togglePause(bool toggle)
    {
        if (pauseUI != null) pauseUI.SetActive(toggle);
        paused = toggle;
        if (toggle)
            Time.timeScale = 0;
        else
        {
            Time.timeScale = 1;
            if (settingsUI != null) settingsUI.SetActive(false);
        }
    }

    /// <summary>
    /// Toggle the pause state of the game
    /// </summary>
    public void togglePause()
    {
        paused = !paused;
        pauseUI.SetActive(paused);
        if (paused)
            Time.timeScale = 0;
        else
        {
            Time.timeScale = 1;
            if (settingsUI != null) settingsUI.SetActive(false);
        }
    }
}
