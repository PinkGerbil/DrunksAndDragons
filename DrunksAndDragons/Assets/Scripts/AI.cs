using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{

    public NavMeshAgent agent;
    public float attackRange;
    public float aggroRange;

    private GameObject[] players;
    PlayerDamageHandler currentPlayer = null;

    [SerializeField]
    [Range(0, 2)]
    float attackTime = 0.5f;
    float attackCountdown;

    [Tooltip("chance that the enemy will drop a pickup 0 for never dropping and 100 for always dropping")]
    [Range(0,100)]
    public int powerUpdropChance;
    public GameObject[] powerUps;


    public GameObject coin;
    public int maxCoinDrop;


    public int health;
    public bool isDead;

    [Header("AI Being Hit")]
    [Tooltip("how long the ai will be stunned for")]
    public float stunPeriod;
    private float stunTime;
    [Tooltip("how much force will be applied to the ai")]
    public float knockbackStrength;
    [Tooltip("how long the ai will be knocked back for")]
    public float knockbackPeriod;
    private float knockbackTime;
    public bool beingHit = false;

    private Vector3 AIHitDir;
    
    new Renderer renderer;

    //boss stuff
    [Header("Boss")]
    public GameObject aoeAttack;

    public float aoeCooldownMin;
    public float aoeCooldownMax;

    private float aoeCooldown;


    public int coinDropAmount;
    public float timeBetweenCoinSpawns;
    private bool coroutineRunning = true;


    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
        players = GameObject.FindGameObjectsWithTag("Player");
        isDead = false;
        FindClosestPlayer();
        

        attackCountdown = attackTime;
        stunTime = stunPeriod;
        stunTime = 0;
        knockbackTime = knockbackPeriod;
        knockbackTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (stunTime > 0)
        {
            if (!isDead)
            {
                if (!agent.isStopped)
                    agent.isStopped = true;
            }
            if (knockbackTime > 0)
            {
                this.transform.position += knockbackStrength * AIHitDir * Time.deltaTime;
                knockbackTime -= Time.deltaTime;
            }
            stunTime -= Time.deltaTime;
        }
        else
        {
            renderer.material.color = Color.white;
            if (!isDead)
            {
                if (agent.isStopped)
                    agent.isStopped = false;
            }
            FindClosestPlayer();
        }
        if (health <= 0 && this.gameObject.name != "Boss(Clone)")
        {
            isDead = true;
            int randomNum = Random.Range(0, 100);
            if(randomNum <= powerUpdropChance)
            {
                int whatPickup = Random.Range(0, powerUps.Length);
                Instantiate(powerUps[whatPickup], this.transform.position, this.transform.rotation);
                //spawn the drop
                //Debug.Log("drop");
            }
            int coinDropAmount = Random.Range(0, maxCoinDrop);
            for(int i = 0; i < coinDropAmount; i++)
            {
                Instantiate(coin, this.transform.position, this.transform.rotation);
            }
            Destroy(this.gameObject);
        }
        //checking to see how many players there are in the scene by seeing how many player tags there are in startup
        else if(stunTime <= 0)
        {
            float distanceToTarget = Vector3.Distance(transform.position, currentPlayer.transform.position);

            if (distanceToTarget > aggroRange || !currentPlayer.Alive)
                FindClosestPlayer();
            else if (distanceToTarget < attackRange)
                attack();
            else
                attackCountdown = attackTime;
        }
        if (this.gameObject.name == "Boss(Clone)")
        {
            if (isDead == true && coroutineRunning == true)
            {
                StartCoroutine(coinDrop());
                coroutineRunning = false;
            }
            if (aoeCooldown <= 0 && !isDead)
            {
                AoEAttack();
                aoeCooldown = Random.Range(aoeCooldownMin, aoeCooldownMax);
            }
            aoeCooldown -= Time.deltaTime;
        }

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
            if (!isDead)
            {
                if (curDistance < closest && !agent.isStopped)
                {
                    closest = curDistance;
                    currentPlayer = child.GetComponent<PlayerDamageHandler>();
                    agent.destination = currentPlayer.transform.position;
                }
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
            Vector3 hitDir = (currentPlayer.transform.position - transform.position);
            hitDir.y = 0;
            currentPlayer.isHitDir += hitDir.normalized;
        }
    }

    public void takeDamage(int damage)
    {
        AIHitDir = (transform.position - currentPlayer.transform.position);
        AIHitDir.y = 0;
        AIHitDir = AIHitDir.normalized;
        renderer.material.color = Color.red;
        health -= damage;
        stunTime = stunPeriod;
        knockbackTime = knockbackPeriod;
        if(health <= 0)
        {
            isDead = true;
        }
    }

    public int getHealth()
    {
        return health;
    }

    IEnumerator coinDrop()
    {
        agent.enabled = false;
        for (int i = 0; i < coinDropAmount; i++)
        {
            int randomRotX = Random.Range(0, 359);
            int randomRotY = Random.Range(0, 359);
            int randomRotZ = Random.Range(0, 359);

            Instantiate(coin, this.transform.position, Quaternion.Euler(randomRotX, randomRotY, randomRotZ));

            yield return new WaitForSeconds(timeBetweenCoinSpawns);
        }
        Destroy(this.gameObject);
    }
    void AoEAttack()
    {
        int maxNum = -1;
        foreach (GameObject child in players)
        {
            maxNum++;
        }
        int playerNum = Random.Range(0, maxNum);
        Instantiate(aoeAttack, players[playerNum].transform.position, this.transform.rotation);
    }

}
