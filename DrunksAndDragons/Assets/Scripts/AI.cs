using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{

    public NavMeshAgent agent;
    public float attackRange;

    private GameObject[] players;
    PlayerDamageHandler currentPlayer = null;

    [SerializeField]
    [Range(0, 2)]
    float attackTime = 0.5f;
    float attackCountdown;
    

    // Start is called before the first frame update
    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        
        FindClosestPlayer();
        

        attackCountdown = attackTime;
    }

    // Update is called once per frame
    void Update()
    {
        //checking to see how many players there are in the scene by seeing how many player tags there are in startup
        float distanceToTarget = Vector3.Distance(transform.position, currentPlayer.transform.position);
        if (distanceToTarget > 5 || !currentPlayer.Alive)
            FindClosestPlayer();
        else if (distanceToTarget < attackRange)
            attack();
        else
            attackCountdown = attackTime;
        
    }

    //getting the closest player to this object if the player moves past a distance set above
    void FindClosestPlayer()
    {
        float closest = Mathf.Infinity;
        foreach (GameObject child in players)
        {
            if (!child.GetComponent<PlayerDamageHandler>().Alive)
                continue;

            float curDistance = Vector3.Distance(transform.position, child.transform.position);
            if (curDistance < closest)
            {
                closest = curDistance;
                currentPlayer = child.GetComponent<PlayerDamageHandler>();
                agent.destination = currentPlayer.transform.position;
            }
            
        }
    }

    //adds a timer to the ai's attack so it doesnt constantly hit the player
    void attack()
    {
        if (attackCountdown > 0)
        {
            attackCountdown -= Time.deltaTime;
        }
        else if(Time.timeScale > 0)
        {
            hitPlayer();
            attackCountdown = attackTime;
        }
    }

    //checks if the target player is alive and not invincible then will hit the player and knock them back 
    public void hitPlayer()
    {
        if (!currentPlayer.Invincible && currentPlayer.Alive)
        {
            currentPlayer.isHit = true;
            currentPlayer.isHitDir += (currentPlayer.transform.position - transform.position).normalized;
        }
    }

}
