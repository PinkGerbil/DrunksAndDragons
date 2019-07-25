using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDamageHandler : MonoBehaviour
{
    [SerializeField] Image HealthPanel;
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


    void Start()
    {
        if (!HealthPanel)
            HealthPanel = GameObject.Find("Health").GetComponent<Image>();
        if (!attackScript)
            attackScript = GetComponent<AttackScript>();

        health = maxHealth;
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
    }

    void LateUpdate()
    {
        if (isHit)
            getHit();
    }

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
