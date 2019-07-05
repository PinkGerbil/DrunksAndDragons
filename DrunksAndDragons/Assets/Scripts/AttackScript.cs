using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    [SerializeField] cameraShake cameraShake;
    [SerializeField] timeStop stopTime;

    [Range(0,5)]
    public float attackDuration = 0.25f;
    float attackCountdown = 0;

    [Range(1, 10)]
    public float attackRange = 2;

    [Range(1, 360)]
    public float attackWidth = 45;

    // Start is called before the first frame update
    void Start()
    {
        if (!stopTime)
            stopTime = GetComponent<timeStop>();
        if (!cameraShake)
            cameraShake = Camera.main.GetComponent<cameraShake>();
        //change attackDuration to be same as attack animation time
    }

    // Update is called once per frame
    void Update()
    {
        if(attackCountdown > 0)
        {
            checkCollision();
            attackCountdown -= Time.deltaTime;
        }
    }

    void checkCollision()
    {
        float timeScalar = 1 / attackDuration;
        Vector3 attackDir = transform.forward;
        attackDir = Quaternion.AngleAxis(attackWidth * 0.5f, Vector3.up) * attackDir;

        attackDir = Quaternion.AngleAxis(-(attackWidth * (1 - attackCountdown * timeScalar)), Vector3.up) * attackDir;

        Vector3 origin = transform.position + attackDir * 0.5f;
        Debug.DrawLine(origin, origin + attackDir * attackRange, Color.green);

        if(Physics.Raycast(origin, attackDir, out RaycastHit hit, attackRange))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Destroy(hit.collider.gameObject);
                cameraShake.enableCamShake();
                stopTime.enableTimeStop();
            }
        }

    }

    public void Attack()
    {
        if(attackCountdown <= 0)
            attackCountdown = attackDuration;
    }
}
