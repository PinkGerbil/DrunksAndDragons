using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class shopPathfinder : MonoBehaviour
{
    private GameObject[] players;
    public GameObject wavespawn;
    public GameObject shopPathfinderObj;
    private bool stopLoop;
    // Start is called before the first frame update



    //spawns the pathfinding ai that guides the players to the shop
    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(wavespawn.GetComponent<WaveSpawn>().shopOpen && !stopLoop)
        {
            //will go through all players and chack if they are alive or in the game in the first place
            foreach (GameObject child in players)
            {
                if (!child.GetComponent<PlayerDamageHandler>().Alive)
                    continue;
                if (!child.activeInHierarchy)
                    continue;
              
                Instantiate(shopPathfinderObj, child.transform.position, child.transform.rotation);
            }
            //for(int i = 0; i < players.Length; i++)
            //{
            //Instantiate(shopPathfinderObj, players[i].transform.position, players[i].rotation);
            //}
            
            stopLoop = true;
        }
        else if(!wavespawn.GetComponent<WaveSpawn>().shopOpen && stopLoop)
        {
            stopLoop = false;
        }
    }
}
