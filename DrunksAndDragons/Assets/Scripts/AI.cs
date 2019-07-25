using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    public GameObject[] players;
    public NavMeshAgent agent;
    public float attackRange;

    private float min1;
    private float min2;
    private float min;

    private float distance0;
    private float distance1;
    private float distance2;
    private float distance3;


    // Start is called before the first frame update
    void Start()
    {
       players = GameObject.FindGameObjectsWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {

        distance0 = Vector3.Distance(agent.transform.position, players[0].transform.position);
        distance1 = Vector3.Distance(agent.transform.position, players[1].transform.position);
        distance2 = Vector3.Distance(agent.transform.position, players[2].transform.position);
        distance3 = Vector3.Distance(agent.transform.position, players[3].transform.position);

        min1 = Mathf.Min(distance0, distance1);
        min2 = Mathf.Min(distance2, distance3);
        min = Mathf.Min(min1, min2);

        if(min > attackRange)
        {
            chase();
        }
        else
        {
            attack();
        }

        
    }

    void chase()
    {
        if (min == distance0)
        {
            agent.destination = players[0].transform.position;
        }
        else if (min == distance1)
        {
            agent.destination = players[1].transform.position;
        }
        else if (min == distance2)
        {
            agent.destination = players[2].transform.position;
        }
        else if (min == distance3)
        {
            agent.destination = players[3].transform.position;
        }
    }

    void attack()
    {
        Debug.Log("attack");
    }
}
