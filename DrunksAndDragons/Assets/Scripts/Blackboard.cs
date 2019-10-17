using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blackboard : MonoBehaviour
{
    
    [SerializeField] Text pauseText;

    [HideInInspector]
    public List<GameObject> players = new List<GameObject>();

    bool paused = false;
    
    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
    }

    /// <summary>
    /// Update is called once each frame
    /// </summary>
    void Update()
    {

        //Debug.Log(players.Count);

    }

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
        pauseText.enabled = toggle;
        paused = toggle;
        if (toggle)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    /// <summary>
    /// Toggle the pause state of the game
    /// </summary>
    public void togglePause()
    {
        paused = !paused;
        pauseText.enabled = !pauseText.enabled;
        if (paused)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }
}
