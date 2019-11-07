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
    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(wavespawn.GetComponent<WaveSpawn>().shopOpen && !stopLoop)
        {
            foreach (GameObject child in players)
            {
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
