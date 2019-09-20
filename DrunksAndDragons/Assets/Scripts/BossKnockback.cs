using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossKnockback : MonoBehaviour
{
    [Tooltip("how long the knockback will stay in scene")]
    public float knockbackDuration = 0.2f;
    private float knockbackCountdown;

    public float radiusIncreaseSpeed = 0.4f;

    // Start is called before the first frame update
    void Start()
    {
        knockbackCountdown = knockbackDuration;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(knockbackCountdown > 0)
        {
            transform.localScale += new Vector3(radiusIncreaseSpeed, 0, radiusIncreaseSpeed);
        }
        else
        {
            Destroy(this.gameObject);
            knockbackCountdown = knockbackDuration;
        }
        knockbackCountdown -= Time.deltaTime;
    }
}
