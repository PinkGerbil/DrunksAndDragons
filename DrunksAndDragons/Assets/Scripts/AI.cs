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

    [SerializeField]
    [Range(0, 2)]
    float attackTime = 0.5f;
    float attackCountdown;

    private int pointAmount = 20;
    

    // Start is called before the first frame update
    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        
        FindClosestPlayer();

        if (!attackScript)
            attackScript = GetComponent<EnemyAttackTest>();

        attackCountdown = attackTime;
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
        else
            attackCountdown = attackTime;
        
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
        if (attackCountdown > 0)
        {
            attackCountdown -= Time.deltaTime;
        }
        else
        {
            attackScript.hitPlayer(currentPlayer.GetComponent<PlayerDamageHandler>());
            attackCountdown = attackTime;
        }
    }

}
