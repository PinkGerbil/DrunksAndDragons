using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackTest : MonoBehaviour
{
    [SerializeField]
    PlayerDamageHandler player;
    
    [Range(1, 10)]
    public float range = 1.5f;

    [SerializeField]
    [Range(0,2)]
    float attackTime = 0.5f;
    float attackCountdown;


    // Start is called before the first frame update
    void Start()
    {
        if (!player)
            player = GameObject.Find("Player").GetComponent<PlayerDamageHandler>();
        attackCountdown = attackTime;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
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
            Debug.DrawLine(transform.position, player.transform.position, Color.red);
        }
        else
        {
            attackCountdown = attackTime;
            Debug.DrawLine(transform.position, player.transform.position, Color.white);
        }
    }

    // if the players is alive and not invincible, tell the player they've been hit
    void hitPlayer()
    {
        if (!player.Invincible && player.Alive)
        {
            player.isHit = true;
            player.isHitDir += (player.transform.position - transform.position).normalized;
            Debug.Log("Hit: " + player.isHitDir);
        }
    }
}
