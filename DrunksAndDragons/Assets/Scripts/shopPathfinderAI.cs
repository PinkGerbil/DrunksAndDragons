using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class shopPathfinderAI : MonoBehaviour
{
    private GameObject shopDestination;
    private float timeBetweenStopNDestroy = 2;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<NavMeshAgent>().destination = GameObject.Find("shopDestination").transform.position;
    }
    
    // Update is called once per frame
    void Update()
    {
        if(gameObject.GetComponent<NavMeshAgent>().remainingDistance < 2)
        {
            if (!gameObject.GetComponent<ParticleSystem>().isStopped)
            {
                gameObject.GetComponent<ParticleSystem>().Stop();
            }
            timeBetweenStopNDestroy -= Time.deltaTime;
            if(timeBetweenStopNDestroy < 0)
            {
                Destroy(gameObject);
            }

        }
    }
}
