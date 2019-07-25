using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackScript : MonoBehaviour
{
    [SerializeField] cameraShake cameraShake;
    [SerializeField] timeStop stopTime;
    [SerializeField] Image AttackPanel;

    public bool IsAttacking { get { return !(sweepCountdown <= 0 && lungeCountdown <= 0); } }

    // consider having multiple attackCooldownDurations depending on what attack was used
    [Range(0, 5)]
    public float attackCooldownDuration = 1.0f;
    float attackCooldown = 0;

    [Range(0,5)]
    public float sweepDuration = 0.25f;
    float sweepCountdown = 0;

    [Range(0, 5)]
    public float lungeDuration = 0.15f;
    float lungeCountdown = 0;
    Vector3 lungeDir;

    [Range(0, 10)]
    public float sweepRange = 2;
    [Range(1, 360)]
    public float sweepWidth = 45;

    [Range(0, 5)]
    public float lungeRange = 0.5f;

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
        if(sweepCountdown > 0)
        {
            checkSweepCollide();
            sweepCountdown -= Time.deltaTime;
        }
        else if(lungeCountdown > 0)
        {
            transform.position += lungeDir * 20 * Time.deltaTime;
            checkLungeCollision();
            lungeCountdown -= Time.deltaTime;
        }
        else if(attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
            AttackPanel.fillAmount = 1 - ((1 / attackCooldownDuration) * attackCooldown);
        }
    }

    void checkSweepCollide()
    {
        float timeScalar = 1 / sweepDuration;
        Vector3 attackDir = transform.forward;
        attackDir = Quaternion.AngleAxis(sweepWidth * 0.5f, Vector3.up) * attackDir;

        attackDir = Quaternion.AngleAxis(-(sweepWidth * (1 - sweepCountdown * timeScalar)), Vector3.up) * attackDir;

        Vector3 origin = transform.position + attackDir * 0.5f;
        Debug.DrawLine(origin, origin + attackDir * sweepRange, Color.green);

        if(Physics.Raycast(origin, attackDir, out RaycastHit hit, sweepRange))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Destroy(hit.collider.gameObject);
                cameraShake.enableCamShake();
                stopTime.enableTimeStop();
            }
        }

    }

    void checkLungeCollision()
    {
        Vector3 rayOrigin = transform.position + lungeDir * 0.5f;
        Debug.DrawLine(rayOrigin, rayOrigin + (lungeDir * lungeRange), Color.red);

        if (Physics.Raycast(rayOrigin, lungeDir, out RaycastHit hit, lungeRange))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Destroy(hit.collider.gameObject);
                cameraShake.enableCamShake();
                stopTime.enableTimeStop();
            }
        }

    }

    public void SweepAttack()
    {
        if (attackCooldown <= 0)
        {
            sweepCountdown = sweepDuration;
            attackCooldown = attackCooldownDuration;
            AttackPanel.fillAmount = 0;
        }
    }

    public void LungeAttack()
    {
        if (attackCooldown <= 0)
        {
            lungeCountdown = lungeDuration;
            attackCooldown = attackCooldownDuration;
            lungeDir = transform.forward;
            AttackPanel.fillAmount = 0;
        }
    }
}
