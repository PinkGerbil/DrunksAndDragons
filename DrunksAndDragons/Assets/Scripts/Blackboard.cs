using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blackboard : MonoBehaviour
{

    int nextPlayerID = 0;
    [SerializeField] Canvas UIcanvas;
    [SerializeField] Text pauseText;

    List<PlayerDamageHandler> players;
    List<Image> AttackPanels;
    List<Image> HealthPanels;

    bool paused = false;

    // Start is called before the first frame update
    void Start()
    {

        players = new List<PlayerDamageHandler>();
        AttackPanels = new List<Image>();
        HealthPanels = new List<Image>();
        if (!UIcanvas)
            UIcanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        if (!pauseText)
            pauseText = UIcanvas.transform.Find("Pause_Text").GetComponent<Text>();
        foreach(Transform child in UIcanvas.transform)
        {
            Image[] images = child.GetComponentsInChildren<Image>();
            foreach(Image image in images)
            {
                if (image.name == "Health")
                    HealthPanels.Add(image);
                else if (image.name == "Stamina")
                    AttackPanels.Add(image);
            }
        }
    }

    /// <summary>
    /// Can be used by AI (if necessary) to find the nearest player.
    /// </summary>
    /// <param name="enemyPos"> the position of the enemy calling this function </param>
    /// <returns> the damage handler for the closest player </returns>
    public PlayerDamageHandler getNearestPlayer(Vector3 enemyPos)
    {
        float distance = Mathf.Infinity;
        PlayerDamageHandler closest = null;
        foreach (PlayerDamageHandler child in players)
        {
            float current = Vector3.Distance(enemyPos, child.transform.position);
            if (current < distance)
            {
                closest = child;
                distance = current;
            }
        }
        return closest.GetComponent<PlayerDamageHandler>();
    }

    /// <summary>
    /// Add player to the players list, then return the ID requested and iterate nextPlayerID
    /// </summary>
    /// <param name="player"> The damage handler of the player calling this function </param>
    /// <returns> The ID of the player requesting their playerID (nextPlayerID) </returns>
    public int GetPlayerID(PlayerDamageHandler player)
    {
        players.Add(player);
        
        return ++nextPlayerID; 
    }

    public Image getHealthUI(int ID)
    {
        return HealthPanels[ID - 1];
    }

    public Image getAttackUI(int ID)
    {
        return AttackPanels[ID - 1];
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
