using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    public NavMeshAgent agent;
    public float attackRange;

    private GameObject[] players;

    private float min1;
    private float min2;
    private float min;

    private float distance0;
    private float distance1;
    private float distance2;
    private float distance3;

    private int playNum;

    // Start is called before the first frame update
    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        playNum = players.Length;
    }

    // Update is called once per frame
    void Update()
    {
        //checking to see how many players there are in the scene by seeing how many player tags there are in startup
        if(playNum == 1)
        {
            onePlayer();
        }
        else if (playNum == 2)
        {
            twoPlayer();
        }
        else if (playNum == 3)
        {
            threePlayer();
        }
        else if (playNum == 4)
        {
            fourPlayer();
        }
    }

    void attack()
    {
        Debug.Log("attack");
    }

    void onePlayer()
    {
        //finds the distance between the ai and the player then decides to chase or attack depending on range
        distance0 = Vector3.Distance(agent.transform.position, players[0].transform.position);

        if (distance0 > attackRange)
        {
            agent.destination = players[0].transform.position;
        }
        else
        {
            attack();
        }

    }
    void twoPlayer()
    {
        distance0 = Vector3.Distance(agent.transform.position, players[0].transform.position);
        distance1 = Vector3.Distance(agent.transform.position, players[1].transform.position);
        min1 = Mathf.Min(distance0, distance1);

        if (min1 > attackRange)
        {
            if (min1 == distance0)
            {
                agent.destination = players[0].transform.position;
            }
            else if (min1 == distance1)
            {
                agent.destination = players[1].transform.position;
            }
        }
        else
        {
            attack();
        }



    }
    void threePlayer()
    {
        distance0 = Vector3.Distance(agent.transform.position, players[0].transform.position);
        distance1 = Vector3.Distance(agent.transform.position, players[1].transform.position);
        distance2 = Vector3.Distance(agent.transform.position, players[2].transform.position);

        min1 = Mathf.Min(distance0, distance1);
        min2 = Mathf.Min(distance2, min1);

        if (min2 > attackRange)
        {
            if (min2 == distance0)
            {
                agent.destination = players[0].transform.position;
            }
            else if (min2 == distance1)
            {
                agent.destination = players[1].transform.position;
            }
            else if (min2 == distance2)
            {
                agent.destination = players[2].transform.position;
            }
        }
        else
        {
            attack();
        }



    }
    void fourPlayer()
    {
        distance0 = Vector3.Distance(agent.transform.position, players[0].transform.position);
        distance1 = Vector3.Distance(agent.transform.position, players[1].transform.position);
        distance2 = Vector3.Distance(agent.transform.position, players[2].transform.position);
        distance3 = Vector3.Distance(agent.transform.position, players[3].transform.position);

        min1 = Mathf.Min(distance0, distance1);
        min2 = Mathf.Min(distance2, distance3);
        min = Mathf.Min(min1, min2);

        if (min > attackRange)
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
        else
        {
            attack();
        }

    }
}
