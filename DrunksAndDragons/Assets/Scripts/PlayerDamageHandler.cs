using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    float knockbackSpeed = 15.0f;

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

    void FixedUpdate()
    {
        int layerMask = 1 << LayerMask.NameToLayer("Floor");
        if (!rigidbody.isKinematic)
        {
            if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 0.2f, layerMask))
            {
                rigidbody.isKinematic = true;
                GetComponent<Collider>().isTrigger = false;
                Vector3 temp = hit.point;
                transform.position = temp;
            }

            Vector3 nextPos = transform.position + rigidbody.velocity * Time.deltaTime;

            if (GetComponent<PlayerMoveScript>().CheckInDirection(nextPos))
            {
                Vector3 temp = Vector3.zero;
                temp.y = rigidbody.velocity.y;
                heldVelocity = rigidbody.velocity;
                heldVelocity.y = 0;
                rigidbody.velocity = temp;
            }
        }

    }

    public bool isKnockedBack { get { return knockbackCountdown > 0; } }

    void Update()
    {
        if (heldVelocity != Vector3.zero)
        {
            rigidbody.velocity += heldVelocity;
            heldVelocity = Vector3.zero;
        }
        if (knockbackCountdown > 0)
        {
            PlayerMoveScript temp = GetComponent<PlayerMoveScript>();
            knockbackCountdown -= Time.deltaTime;
            if (!temp.CheckInDirection(transform.position + knockbackSpeed * isHitDir.normalized * Time.deltaTime))
            {
                transform.position += knockbackSpeed * isHitDir.normalized * Time.deltaTime;
                temp.checkGrounded();
            }
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

        if(lives <= 0)
        {
            HealthPanel.fillAmount = 0;
        }

        if (!Alive && respawnCountdown <= 0)
        {
            lives--;
            Debug.Log("ded");
            respawnCountdown = respawnTime;
            if (gameObject.GetComponent<AttackScript>().loseHealthUpOnDeath)
            {
                maxHealth = originalMaxHealth;
            }
            transform.Find("group1").GetComponent<SkinnedMeshRenderer>().enabled = false;
            transform.Find("polySurface3").GetComponent<SkinnedMeshRenderer>().enabled = false;
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
                transform.Find("polySurface3").GetComponent<SkinnedMeshRenderer>().enabled = true;
                Player_Icon.GetComponent<SpriteRenderer>().enabled = true;
                GetComponent<PlayerMoveScript>().enabled = true;
                transform.position = spawnLocation;
            }
        }
    }

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
        if (animator != null)
            animator.SetTrigger("WasHit");
        isHitDir.y = 0;
        isHitDir.Normalize();
        knockbackCountdown = knockbackTime;
        rigidbody.isKinematic = false;
        IFrameTime = IFrames;
        health -= 1;
        isHit = false;
    }

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
