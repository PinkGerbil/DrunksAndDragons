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

    // Start is called before the first frame update
    void Start()
    {
    }

    void Update()
    {

        //Debug.Log(players.Count);

    }

    public void addPlayer(GameObject player)
    {
        players.Add(player);
    }
    
    

    public void togglePause(bool toggle)
    {
        pauseText.enabled = toggle;
        paused = toggle;
        if (toggle)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

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
