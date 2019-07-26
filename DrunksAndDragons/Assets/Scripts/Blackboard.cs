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

    // Update is called once per frame
    void Update()
    {
        
    }

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

    public int GetPlayerID(PlayerDamageHandler player)
    {
        Debug.Log(player.name);
        players.Add(player);
        
        Debug.Log(nextPlayerID);
        return ++nextPlayerID; 
    }


}
