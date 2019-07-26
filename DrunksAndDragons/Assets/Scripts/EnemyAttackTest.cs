using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackTest : MonoBehaviour
{
    // consider just using some of this code in ai and deleting this script

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
    [Tooltip("How far away the player target needs to be before deciding to switch targets")]
    [Range(0,100)]
    float maxFollowRange = 6;

    // Start is called before the first frame update
    void Start()
    {
        
        attackCountdown = attackTime;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // if the players is alive and not invincible, tell the player they've been hit



}
