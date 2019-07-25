using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageHandler : MonoBehaviour
{
    public float Invincible { get { return IFrameTime = 0; } }

    float IFrames = 3.0f;
    float IFrameTime = 0;

    public bool isHit = false;
    public Vector3 isHitDir = Vector3.zero;

    [SerializeField]
    float knockbackTime = 0.2f;
    float knockbackCountdown = 0;

    [SerializeField]
    float knockbackForce = 5.0f;

    public int health = 6;

    void lateUpdate()
    {
        if (isHit)
            getHit();

        if (knockbackCountdown > 0)
        {
            knockbackCountdown -= Time.deltaTime;
            transform.position += knockbackForce * isHitDir.normalized;
        }
    }

    void getHit()
    {
        isHitDir.Normalize();
        isHitDir.y = 0;
        hitToward(); //possible seperate script attached to player
                                        // responsible for moving player with "knockback"
        IFrameTime = IFrames;
        health -= 1;
        isHit = false;
    }

    void hitToward()
    {
        knockbackCountdown = knockbackTime;
    }
}
