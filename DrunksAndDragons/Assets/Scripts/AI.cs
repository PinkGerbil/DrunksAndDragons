using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyAttackTest))]
public class AI : MonoBehaviour
{
    [SerializeField]
    EnemyAttackTest attackScript;

    public NavMeshAgent agent;
    public float attackRange;

    private GameObject[] players;
    GameObject currentPlayer = null;

    private float min1;
    private float min2;
    private float min;

    private float distance0;
    private float distance1;
    private float distance2;
    private float distance3;

    private int pointAmount = 20;

    private int playNum;

    // Start is called before the first frame update
    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        playNum = players.Length;
        FindClosestPlayer();

        if (!attackScript)
            attackScript = GetComponent<EnemyAttackTest>();
    }

    // Update is called once per frame
    void Update()
    {
        //checking to see how many players there are in the scene by seeing how many player tags there are in startup
        float distanceToTarget = Vector3.Distance(transform.position, currentPlayer.transform.position);
        if (distanceToTarget > 5)
            FindClosestPlayer();
        else if (distanceToTarget < attackRange)
            attack();
        
    }

    void FindClosestPlayer()
    {
        float closest = Mathf.Infinity;
        foreach (GameObject child in players)
        {
            float curDistance = Vector3.Distance(transform.position, child.transform.position);
            if (curDistance < closest)
            {
                closest = curDistance;
                currentPlayer = child;
                agent.destination = currentPlayer.transform.position;
            }
        }
    }

    void attack()
    {
        attackScript.hitPlayer(currentPlayer.GetComponent<PlayerDamageHandler>());
        //Debug.Log("attack");
    }

}
