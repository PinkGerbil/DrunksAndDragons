using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AttackScript))]
public class PlayerDamageHandler : MonoBehaviour
{
    [SerializeField]
    [Tooltip("HealthPanel should be a panel in the UI with a horizontal fill method")]
    Image HealthPanel;
    [SerializeField] AttackScript attackScript;

    public bool Invincible { get { return IFrameTime > 0 || attackScript.IsAttacking; } }
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


    Rigidbody rigidbody;

    void Start()
    {
        if (!HealthPanel)
            HealthPanel = GameObject.Find("Health").GetComponent<Image>();
        if (!attackScript)
            attackScript = GetComponent<AttackScript>();

        health = maxHealth;

        rigidbody = GetComponent<Rigidbody>();

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
        
        HealthPanel.fillAmount = (1.0f / maxHealth) * health;

        if (health <= 0)
            Debug.Log("ded");

        if (!rigidbody.isKinematic && Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 1))
        {
            if (hit.collider.CompareTag("Environment"))
            {
                rigidbody.isKinematic = true;
                Vector3 temp = transform.position;
                temp.y = 1;
                transform.position = temp;
            }
        }
    }

    void LateUpdate()
    {
        if (isHit)
            getHit();
    }


    // recieve damage and get knocked back, activate invincibility frames
    void getHit()
    {
        isHitDir.Normalize();
        isHitDir.y = 0;
        Debug.Log(isHitDir);
        hitToward(); 
        IFrameTime = IFrames;
        health -= 1;
        isHit = false;
    }

    void hitToward()
    {
        knockbackCountdown = knockbackTime;
    }
}
