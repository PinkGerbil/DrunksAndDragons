using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackboard : MonoBehaviour
{

    int nextPlayerID = 0;
    List<PlayerDamageHandler> players;
    

    // Start is called before the first frame update
    void Start()
    {
        
        players = new List<PlayerDamageHandler>();
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
        Debug.Log(player.name);
        players.Add(player);
        
        Debug.Log(nextPlayerID);
        return ++nextPlayerID; 
    }


}
