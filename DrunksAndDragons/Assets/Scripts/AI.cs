using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator), typeof(NavMeshAgent))]
public class AI : MonoBehaviour
{
    Animator animator;
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
    public int minCoinDrop;
    public int maxCoinDrop;

    public int maxHealth;
    private int health;
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
    private bool hasTarget;
    private int skippedPlayers = 0;

    private Vector3 AIHitDir;
    
    new Renderer renderer;

    //boss stuff
    [Header("Boss")]
    public GameObject boss;

    //aoe attack
    [Header("aoe attack")]
    [Tooltip("allows the boss to do this action or not")]
    public bool aoeOn;
    public GameObject aoeAttack;
    public float aoeCooldownMin;
    public float aoeCooldownMax;
    private float aoeCooldown;

    //a damaging trail 
    [Header("damaging trail")]
    [Tooltip("allows the boss to do this action or not")]
    public bool trailOn;
    public float trailCooldown;
    private float trailCD;

    //knockback attack
    [Header("knockback attack")]
    [Tooltip("allows the boss to do this action or not")]
    public bool knockbackOn;
    public GameObject knockback;
    [Tooltip("how close the players have to be for the boss to do the knockback")]
    public float knockbackCheckRange;
    private int withinRange;
    public float knockbackAttackCooldown;
    private float knockbackAttackCD;

    //healing ability
    [Header("healing ability")]
    [Tooltip("allows the boss to do this action or not")]
    public bool healingOn;
    [Tooltip("how long the boss channels the heal for")]
    public float channelDuration;
    private float channelTimer;
    public ParticleSystem channelingParticle;
    [Tooltip("leave off")]
    public bool channeling;
    [Tooltip("distance the ai has to be to the boss to heal it")]
    public float healRange;
    [Tooltip("how much each ai will heal the boss for")]
    public int healAmount;
    [Header("coin drop")]
    public int coinDropAmount;
    public float timeBetweenCoinSpawns;
    private bool coroutineRunning = true;


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        renderer = transform.Find("default2").GetComponent<Renderer>();
        players = GameObject.FindGameObjectsWithTag("Player");
        isDead = false;
        FindClosestPlayer();

        boss = GameObject.Find("Boss(Clone)");

        attackCountdown = attackTime;
        stunTime = stunPeriod;
        stunTime = 0;
        knockbackTime = knockbackPeriod;
        knockbackTime = 0;
        withinRange = 0;
        health = maxHealth;
        channelTimer = channelDuration;
    }

    // Update is called once per frame
    void Update()
    { 

        //if the boss is channeling for healing basic ai will go to him
        if(boss != null && boss.GetComponent<AI>().channeling && this.gameObject.name != "Boss(Clone)")
        {
            agent.destination = boss.transform.position;
        }
        //when basic ai is withing set range they will die and heal the boss
        if(boss != null && boss.GetComponent<AI>().channeling && Vector3.Distance(this.transform.position, boss.transform.position) < healRange && this.gameObject.name != "Boss(Clone)")
        {
            boss.GetComponent<AI>().heal(healAmount);
            Destroy(this.gameObject);
        }
        if (stunTime > 0)
        {
            if (!isDead)
            {
                if (agent.enabled)
                {
                    if (!agent.isStopped)
                        agent.isStopped = true;
                }
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
            if (renderer != null)
            {
                renderer.material.color = Color.white;
            }
            if (!isDead)
            {
                if (agent.enabled)
                {
                    if (agent.isStopped)
                        agent.isStopped = false;
                }
            }
            if (boss != null && !boss.GetComponent<AI>().channeling)
            {
                FindClosestPlayer();
            }
            else if(boss == null)
            {
                FindClosestPlayer();
            }
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
            int coinDropAmount = Random.Range(minCoinDrop, maxCoinDrop);
            for(int i = 0; i < coinDropAmount; i++)
            {
                Instantiate(coin, this.transform.position + new Vector3(0,2,0), this.transform.rotation);
            }
            Destroy(this.gameObject);
        }
        //checking to see how many players there are in the scene by seeing how many player tags there are in startup
        else if(boss != null && stunTime <= 0 && !boss.GetComponent<AI>().channeling && !isDead && hasTarget)
        {
            agent.isStopped = false;

            float distanceToTarget = Vector3.Distance(transform.position, currentPlayer.transform.position);

            if (distanceToTarget > aggroRange || !currentPlayer.Alive)
                FindClosestPlayer();
            else if (distanceToTarget < attackRange)
                attack();
            else
                attackCountdown = attackTime;
        }
        else if (boss == null && stunTime <= 0 && !isDead && hasTarget)
        {
            agent.isStopped = false;

            float distanceToTarget = Vector3.Distance(transform.position, currentPlayer.transform.position);

            if (distanceToTarget > aggroRange || !currentPlayer.Alive)
                FindClosestPlayer();
            else if (distanceToTarget < attackRange)
                attack();
            else
                attackCountdown = attackTime;
        }
        else if (!hasTarget && !isDead || !currentPlayer.Alive)
        {
            agent.isStopped = true;
        }
        //boss things
        if (this.gameObject.name == "Boss(Clone)")
        {
            //when dead will drop lots of coins in random directions 
            if (isDead == true && coroutineRunning == true)
            {
                StartCoroutine(coinDrop());
                coroutineRunning = false;
            }
            //picks a random player and puts a damaging aoe at their feet and does damage every set amount of time in aoe.cs
            if (aoeCooldown <= 0 && !isDead && aoeOn)
            {
                AoEAttack();
                aoeCooldown = Random.Range(aoeCooldownMin, aoeCooldownMax);
            }
            //leaves damaging aoe behind the boss each time this if is true
            if(trailCD <= 0 && !isDead && trailOn)
            {
                bossTrailAttack();
                trailCD = trailCooldown;
            }
            //wip knockback attack that when multiple players are close to the boss he will knock them back
            KnockbackAttackCheck();
            if(withinRange >= 2 && knockbackAttackCD <= 0 && knockbackOn && !isDead)
            {
                KnockbackAttack();
                knockbackAttackCD = knockbackAttackCooldown;
            }
            //when the boss is at half health or less he will start channeling that will call all basic ai to him where they will die to heal him
            if(health <= maxHealth/2 && channelTimer > 0 && !isDead && healingOn)
            {
                channeling = true;
                channelingParticle.Play();
            }
            if(channeling)
            {
                if(isDead)
                {
                    channeling = false;
                }
                channelTimer -= Time.deltaTime;
                if (channelTimer <= 0)
                {
                    channelingParticle.Stop();
                    channeling = false;
                }
            }

            withinRange = 0;
            aoeCooldown -= Time.deltaTime;
            trailCD -= Time.deltaTime;
            knockbackAttackCD -= Time.deltaTime;
        }
        if(animator != null)
        {
            if (Vector3.Distance(agent.pathEndPosition, transform.position) <= 0.1f || agent.isStopped)
                animator.SetBool("Moving", false);
            else
                animator.SetBool("Moving", true);
        }
        //if(!agent.hasPath)
        //{
        //    hasTarget = false;
        //}


    }

    //getting the closest player to this object if the player moves past a distance set above
    void FindClosestPlayer()
    {
        float closest = Mathf.Infinity;
        foreach (GameObject child in players)
        {
            if (!child.GetComponent<PlayerDamageHandler>().Alive)
                continue;

            //meant to skip the player if there isnt a path that can be made
            NavMeshPath testPath = new NavMeshPath();
            agent.CalculatePath(child.transform.position, testPath);
            if(testPath.status == NavMeshPathStatus.PathPartial)
            {
                skippedPlayers++;
                continue;
            }


            float curDistance = Vector3.Distance(transform.position, child.transform.position);
            if (!isDead)
            {
                if (curDistance < closest && !agent.isStopped)
                {
                    hasTarget = true;
                    closest = curDistance;
                    currentPlayer = child.GetComponent<PlayerDamageHandler>();
                    agent.destination = currentPlayer.transform.position;
                }
            }
            
        }
        if(skippedPlayers >= players.Length)
        {
            hasTarget = false;
        }
        skippedPlayers = 0;

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
            if (animator != null) animator.SetTrigger("Hit");
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
        if (renderer != null) renderer.material.color = Color.red;
        if (animator != null) animator.SetTrigger("GetHit");
        health -= damage;
        stunTime = stunPeriod;
        knockbackTime = knockbackPeriod;
        if(health <= 0)
        {
            isDead = true;
        }
    }

    public void takeDamage(int damage, Vector3 dir)
    {
        AIHitDir = dir;
        if (renderer != null) renderer.material.color = Color.red;
        if (animator != null) animator.SetTrigger("GetHit");
        health -= damage;
        stunTime = stunPeriod;
        knockbackTime = knockbackPeriod;
        if (health <= 0)
        {
            isDead = true;
        }
    }

    public void heal(int healingAmount)
    {
        health += healAmount;
    }

    public int getHealth()
    {
        return health;
    }

    //spawns coins facing random directions to then be launched by the start function on coin.cs
    IEnumerator coinDrop()
    {
        this.gameObject.GetComponent<Collider>().enabled = false;
        agent.isStopped = true;
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
    //puts a aoe on the feet of a random player to deal damage to them and whoever stays standing in it
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

    void bossTrailAttack()
    {
        Instantiate(aoeAttack, this.transform.position - new Vector3(0,1.25f,0), this.transform.rotation);
    }

    //checks to see how many players are within a set range
    void KnockbackAttackCheck()
    {
        foreach (GameObject child in players)
        {
            if (!child.GetComponent<PlayerDamageHandler>().Alive)
                continue;
            float distance = Vector3.Distance(transform.position, child.transform.position);
            if (distance <= knockbackCheckRange)
            {
                withinRange++;
            }
        }
        
    }
    //knocks back players who are close
    void KnockbackAttack()
    {
        animator.SetTrigger("Blast");
        Instantiate(knockback,transform.position,transform.rotation);
    }


}
