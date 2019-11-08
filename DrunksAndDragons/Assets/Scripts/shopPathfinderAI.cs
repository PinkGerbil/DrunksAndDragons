using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



//just pathfinds towards the shop leaving particles, guiding players to the shop
public class shopPathfinderAI : MonoBehaviour
{
    private GameObject shopDestination;
    private float timer = 8;
    private float timeBetweenStopNDestroy = 2;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<NavMeshAgent>().destination = GameObject.Find("shopDestination").transform.position;
    }
    
    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        //stops emmiting
        if(timer < 0)
        {
            if (!gameObject.GetComponent<ParticleSystem>().isStopped)
            {
                gameObject.GetComponent<ParticleSystem>().Stop();
            }
            timeBetweenStopNDestroy -= Time.deltaTime;
            //after the emmision is stopped it waits a little bit before fully deleting the emmiter
            if(timeBetweenStopNDestroy < 0)
            {
                Destroy(gameObject);
            }

        }
    }
}
