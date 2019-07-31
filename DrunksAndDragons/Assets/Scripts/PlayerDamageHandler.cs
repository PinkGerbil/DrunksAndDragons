using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AttackScript), typeof(Rigidbody))]
public class PlayerDamageHandler : MonoBehaviour
{

    [SerializeField] Rigidbody rigidbody;
    [SerializeField] AttackScript attackScript;
    [SerializeField]
    [Tooltip("HealthPanel should be a panel in the UI with a horizontal fill method")]
    public Image HealthPanel;

    public bool Invincible { get { return !(IFrameTime <= 0 && !attackScript.IsAttacking && rigidbody.isKinematic); } }
    public bool Alive { get { return health > 0; } }

    [Range(1, 10)]
    float IFrames = 0.5f;
    float IFrameTime = 0;

    public bool isHit = false;
    public Vector3 isHitDir = Vector3.zero;

    [Range(0,5)]
    float knockbackTime = 0.2f;
    float knockbackCountdown = 0;

    [Range(1,100)]
    float knockbackForce = 25;

    [Range(1,10)]
    public int maxHealth = 6;
    public int health;


    void Start()
    {
        if (!attackScript)
            attackScript = GetComponent<AttackScript>();

        health = maxHealth;

        if(!rigidbody)
            rigidbody = GetComponent<Rigidbody>();

        if(rigidbody != null)
            rigidbody.isKinematic = true;

    }

    void Update()
    {
        if (knockbackCountdown > 0)
        {
            knockbackCountdown -= Time.deltaTime;
            transform.position += knockbackForce * isHitDir.normalized * Time.deltaTime;
        }
        else if (IFrameTime > 0)
        {
            //Debug.Log(IFrameTime);
            isHitDir = Vector3.zero;
            IFrameTime -= Time.deltaTime;
            if (IFrameTime < 0) IFrameTime = 0;
        }


        if (!rigidbody.isKinematic && Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 1))
        {
            if (hit.collider.CompareTag("Environment"))
            {
                rigidbody.isKinematic = true;

                // set the player position to just above where the ray hit
                Vector3 temp = hit.point;
                temp.y = 1;
                transform.position = temp;
            }
        }

        if(HealthPanel != null)
            HealthPanel.fillAmount = (1.0f / maxHealth) * health;

        if (Alive && health <= 0)
        {
            Debug.Log("ded");
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
        Debug.Log(isHitDir);
        knockbackCountdown = knockbackTime;
        IFrameTime = IFrames;
        health -= 1;
        isHit = false;
    }
}
