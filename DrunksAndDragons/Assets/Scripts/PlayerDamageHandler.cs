using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AttackScript), typeof(Rigidbody), typeof(Animator))]
public class PlayerDamageHandler : MonoBehaviour
{
    [SerializeField]
    public Animator animator;
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] AttackScript attackScript;
    [SerializeField]
    [Tooltip("HealthPanel should be a panel in the UI with a horizontal fill method")]
    public Image HealthPanel;

    public bool Invincible { get { return !(IFrameTime <= 0 && !attackScript.IsAttacking && rigidbody.isKinematic); } }
    public bool Alive { get { return health > 0; } }

    [SerializeField]
    [Range(1, 10)]
    float IFrames = 0.5f;
    float IFrameTime = 0;

    public bool isHit = false;
    public Vector3 isHitDir = Vector3.zero;

    [SerializeField]
    [Range(0,5)]
    float knockbackTime = 0.2f;
    float knockbackCountdown = 0;

    [SerializeField]
    [Range(1,100)]
    float knockbackForce = 15.0f;

    [SerializeField]
    [Range(1,10)]
    public int maxHealth = 6;
    public int health;

    Vector3 spawnLocation;

    [SerializeField]
    float respawnTime = 5;
    float respawnCountdown = 0;

    void Start()
    {
        if (!animator)
            animator = GetComponent<Animator>();
        spawnLocation = transform.position;
        if (!attackScript)
            attackScript = GetComponent<AttackScript>();

        health = maxHealth;

        if(!rigidbody)
            rigidbody = GetComponent<Rigidbody>();

        if(rigidbody != null)
            rigidbody.isKinematic = true;

        if (HealthPanel != null)
            HealthPanel.transform.parent.gameObject.SetActive(gameObject.activeInHierarchy);
       
    }

    void Update()
    {

        if (knockbackCountdown > 0)
        {
            PlayerMoveScript temp = GetComponent<PlayerMoveScript>();
            knockbackCountdown -= Time.deltaTime;
            if(!temp.checkInFront(transform.position + knockbackForce * isHitDir.normalized * Time.deltaTime))
                transform.position += knockbackForce * isHitDir.normalized * Time.deltaTime;
            temp.checkGrounded();

        }
        else if (IFrameTime > 0)
        {
            //Debug.Log(IFrameTime);
            isHitDir = Vector3.zero;
            IFrameTime -= Time.deltaTime;
            rigidbody.isKinematic = true;
            if (IFrameTime < 0) IFrameTime = 0;
        }


        if (!rigidbody.isKinematic && Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 1))
        {
            if (hit.collider.CompareTag("Environment"))
            {
                rigidbody.isKinematic = true;
                GetComponent<Collider>().isTrigger = false;
                // set the player position to just above where the ray hit
                Vector3 temp = hit.point;
                //temp.y += 1;
                transform.position = temp;
            }
        }

        if(HealthPanel != null && Alive)
            HealthPanel.fillAmount = (1.0f / maxHealth) * health;

        if (!Alive && respawnCountdown <= 0)
        {
            Debug.Log("ded");
            respawnCountdown = respawnTime;
            transform.Find("group1").GetComponent<SkinnedMeshRenderer>().enabled = false;
            transform.Find("polySurface3").GetComponent<SkinnedMeshRenderer>().enabled = false;
            //GetComponent<CapsuleCollider>().enabled = false;
            //delete the following line when proper model is added
        }
        else if(respawnCountdown > 0)
        {
            respawnCountdown -= Time.deltaTime;
            if(HealthPanel != null)
            HealthPanel.fillAmount = 1.0f - (respawnCountdown / respawnTime);
            if (respawnCountdown <= 0)
            {
                health = maxHealth;
                transform.Find("group1").GetComponent<SkinnedMeshRenderer>().enabled = true;
                transform.Find("polySurface3").GetComponent<SkinnedMeshRenderer>().enabled = true;
                transform.position = spawnLocation;
                //delete the following line when proper model is added
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
        isHitDir.Normalize();
        isHitDir.y = 0;
        knockbackCountdown = knockbackTime;
        rigidbody.isKinematic = false;
        IFrameTime = IFrames;
        health -= 1;
        isHit = false;
    }
}
