using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

[RequireComponent(typeof(AttackScript), typeof(Rigidbody), typeof(Animator))]
public class PlayerDamageHandler : MonoBehaviour
{
    [SerializeField]
    public Animator animator;
    [SerializeField] new Rigidbody rigidbody;
    [SerializeField] AttackScript attackScript;
    [SerializeField]
    [Tooltip("HealthPanel should be a panel in the UI with a horizontal fill method")]
    public Image HealthPanel;
    public Text LivesCounter;
    public GameObject Player_Icon;

    public bool Invincible { get { return !(IFrameTime <= 0 && !attackScript.IsAttacking && rigidbody.isKinematic); } }
    public bool Alive { get { return health > 0; } }

    [SerializeField]
    [Range(1, 10)]
    float IFrames = 0.5f;
    float IFrameTime = 0;

    public bool isHit = false;
    public Vector3 isHitDir = Vector3.zero;

    [SerializeField]
    [Tooltip("How long the knockback takes effect")]
    [Range(0,5)]
    float knockbackTime = 0.2f;
    float knockbackCountdown = 0;

    [SerializeField]
    [Tooltip("How fast the player moves during a knockback")]
    [Range(1,100)]
    float knockbackSpeed = 5.0f;

    [SerializeField]
    [Tooltip("The max health of the player")]
    [Range(1,10)]
    public int maxHealth = 6;
    private int originalMaxHealth;
    public int health;
    public int lives;

    Vector3 spawnLocation;

    [SerializeField]
    float respawnTime = 5;
    float respawnCountdown = 0;

    Vector3 heldVelocity = Vector3.zero;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        if (!animator)
            animator = GetComponent<Animator>();
        spawnLocation = transform.position;
        if (!attackScript)
            attackScript = GetComponent<AttackScript>();
        originalMaxHealth = maxHealth;
        health = maxHealth;

        if(!rigidbody)
            rigidbody = GetComponent<Rigidbody>();

        if(rigidbody != null)
            rigidbody.isKinematic = true;

        if (HealthPanel != null)
            HealthPanel.transform.parent.gameObject.SetActive(gameObject.activeInHierarchy);
       
    }

    /// <summary>
    /// Fixed Update is called in a fixed timestep
    /// </summary>
    void FixedUpdate()
    {
        int layerMask = 1 << LayerMask.NameToLayer("Floor");
        if (!rigidbody.isKinematic)
        {
            if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 0.2f, layerMask))
            {
                rigidbody.isKinematic = true;
                rigidbody.useGravity = false;
                GetComponent<Collider>().isTrigger = false;
                
                Vector3 temp = hit.point;
                transform.position = temp;
                //GetComponent<PlayerInput>().stopVibrate();
                GetComponent<PlayerInput>().setVibration(1, 0.2f);
            }

            Vector3 velocity = rigidbody.velocity;
            string wallTag = null;
            while (GetComponent<PlayerMoveScript>().CheckInDirection(velocity, out Vector3 colNorm, out string tag))
            {
                velocity = Vector3.ProjectOnPlane(velocity, colNorm);
                wallTag = tag;
            }
            
            if(wallTag == "Environment")
            {
                transform.position += -Vector3.right * 5 * Time.fixedDeltaTime;
            }
            rigidbody.velocity = velocity;
        }

    }

    /// <summary>
    /// Checks to see if the player has been knocked back
    /// </summary>
    public bool isKnockedBack { get { return knockbackCountdown > 0; } }

    /// <summary>
    /// Update is called before every fram update
    /// </summary>
    void Update()
    {
        if (knockbackCountdown > 0)
        {
            PlayerMoveScript playerMove = GetComponent<PlayerMoveScript>();
            knockbackCountdown -= Time.deltaTime;
            Vector3 velocity = knockbackSpeed * isHitDir.normalized * Time.deltaTime;
            for (int i = 0; playerMove.CheckInDirection(velocity, out Vector3 colNorm) || i < 99; i++)
            {
                velocity = Vector3.ProjectOnPlane(velocity, colNorm);
            }
            velocity.y = 0;
            transform.position += velocity;
            playerMove.checkGrounded();
            //if (knockbackCountdown <= 0)
            //  GetComponent<PlayerInput>().startVibrate(0);
        }
        else if (IFrameTime > 0)
        {
            isHitDir = Vector3.zero;
            IFrameTime -= Time.deltaTime;
            rigidbody.isKinematic = true;
            if (IFrameTime < 0) IFrameTime = 0;
        }


        if(HealthPanel != null && Alive)
            HealthPanel.fillAmount = (1.0f / maxHealth) * health;

        if(lives <= 0 && HealthPanel != null)
        {
            HealthPanel.fillAmount = 0;
        }

        if (!Alive && respawnCountdown <= 0)
        {
            lives--;

            respawnCountdown = respawnTime;
            if (gameObject.GetComponent<AttackScript>().loseHealthUpOnDeath)
            {
                maxHealth = originalMaxHealth;
            }
            transform.Find("group1").GetComponent<SkinnedMeshRenderer>().enabled = false;
            Player_Icon.GetComponent<SpriteRenderer>().enabled = false; 
            GetComponent<PlayerMoveScript>().enabled = false; 
        }
        else if(respawnCountdown > 0 && lives > 0)
        {
            respawnCountdown -= Time.deltaTime;
            if(HealthPanel != null)
            HealthPanel.fillAmount = 1.0f - (respawnCountdown / respawnTime);
            if (respawnCountdown <= 0)
            {
                health = maxHealth;
                transform.Find("group1").GetComponent<SkinnedMeshRenderer>().enabled = true;
                Player_Icon.GetComponent<SpriteRenderer>().enabled = true;
                GetComponent<PlayerMoveScript>().enabled = true;
                transform.position = spawnLocation;
            }
        }

        if(LivesCounter != null) LivesCounter.text = lives.ToString(); 
    }

    /// <summary>
    /// Late Update is called after all other updates have finished their processes
    /// </summary>
    void LateUpdate()
    {
        if (isHit)
            getHit();
    }


    /// <summary>
    /// Get direction of knockback then take damage and start knockback
    /// </summary>
    void getHit()
    {
        //GamePad.SetVibration((PlayerIndex)GetComponent<PlayerInput>().controller, 100, 100);
        if (animator != null)
            animator.SetTrigger("WasHit");
        isHitDir.y = 0;
        isHitDir.Normalize();
        knockbackCountdown = knockbackTime;
        rigidbody.isKinematic = false;
        IFrameTime = IFrames;
        health -= 1;
        isHit = false;
        GetComponent<PlayerInput>().setVibration(0.5f, 0.25f);
    }

    /// <summary>
    /// Checks to see if the player has entered a damage collider
    /// </summary>
    /// <param name="other">Collider</param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "aoe")
        {
            getHit();
        }
        
        if(other.gameObject.tag == "knockback")
        {
            Vector3 hitDir = (transform.position - other.transform.position);
            hitDir.y = 0;
            isHitDir += hitDir.normalized;

            getHit();
        }
    }
}
