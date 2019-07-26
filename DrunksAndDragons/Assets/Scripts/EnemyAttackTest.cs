using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackTest : MonoBehaviour
{
    [SerializeField]
    Blackboard blackboard;

    [SerializeField]
    [Tooltip("Set this if there is no blackboard")]
    PlayerDamageHandler currentPlayer;
    
    [Range(1, 10)]
    public float range = 1.5f;

    [SerializeField]
    [Range(0,2)]
    float attackTime = 0.5f;
    float attackCountdown;

    [SerializeField]
    [Range(0,100)]
    float maxFollowRange = 6;

    // Start is called before the first frame update
    void Start()
    {
        if(!blackboard)
            blackboard = GameObject.Find("Game Manager").GetComponent<Blackboard>();
        
        attackCountdown = attackTime;
    }

    // Update is called once per frame
    void Update()
    {
        if(!currentPlayer && blackboard != null)
            currentPlayer = blackboard.getNearestPlayer(transform.position);
        float distance = Vector3.Distance(transform.position, currentPlayer.transform.position);
        if (distance < range)
        {
            if (attackCountdown > 0)
            {
                attackCountdown -= Time.deltaTime;
            }
            else
            {
                hitPlayer();
            }
            Debug.DrawLine(transform.position, currentPlayer.transform.position, Color.red);
        }
        else
        {
            attackCountdown = attackTime;
            Debug.DrawLine(transform.position, currentPlayer.transform.position, Color.white);
        }
        if ((distance > maxFollowRange || !currentPlayer.Alive) && blackboard != null)
        {
            currentPlayer = blackboard.getNearestPlayer(transform.position);
        }
    }

    // if the players is alive and not invincible, tell the player they've been hit
    void hitPlayer()
    {
        if (!currentPlayer.Invincible && currentPlayer.Alive)
        {
            currentPlayer.isHit = true;
            currentPlayer.isHitDir += (currentPlayer.transform.position - transform.position).normalized;
            Debug.Log("Hit: " + currentPlayer.isHitDir);
        }
    }
}
