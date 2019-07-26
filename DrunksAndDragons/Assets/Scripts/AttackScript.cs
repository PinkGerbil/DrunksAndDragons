using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(timeStop))]
public class AttackScript : MonoBehaviour
{
    const float playerWidth = 0.5f;

    [SerializeField] cameraShake cameraShake;
    [SerializeField] timeStop stopTime;

    [SerializeField]
    [Tooltip("AttackPanel should be a panel in the UI with a horizontal fill method")]
    Image AttackPanel;

    public bool IsAttacking { get { return !(sweepCountdown <= 0 && lungeCountdown <= 0); } }

    // consider having multiple attackCooldownDurations depending on what attack was used
    [Range(0, 5)]
    public float attackCooldownDuration = 1.0f;
    float attackCooldown = 0;

    [Tooltip("How fast the sweep occurs")]
    [Range(0,5)]
    public float sweepDuration = 0.25f;
    float sweepCountdown = 0;

    [Tooltip("How long the lunge attack lasts")]
    [Range(0, 5)]
    public float lungeDuration = 0.15f;
    float lungeCountdown = 0;
    Vector3 lungeDir;

    [Tooltip("The range of the sweep attack")]
    [Range(0, 10)]
    public float sweepRange = 2;
    [Tooltip("How wide the sweep attack should be")]
    [Range(1, 360)]
    public float sweepWidth = 45;

    [Tooltip("How far in front the rays of the lunge attack reach")]
    [Range(0, 5)]
    public float lungeRange = 0.5f;

    [Tooltip("How far away the player can grab another player from")]
    [Range(0, 5)]
    public float grabRange = 1.5f;
    [Tooltip("The angle of the grab check in front of the player")]
    [Range(1, 360)]
    public float grabWidth = 45.0f;

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
            if(AttackPanel != null)
                AttackPanel.fillAmount = 1 - ((1 / attackCooldownDuration) * attackCooldown);
        }
    }

    // fire out a ray based on how long the attack has been active
    void checkSweepCollide()
    {
        float timeScalar = 1 / sweepDuration;
        Vector3 attackDir = transform.forward;
        attackDir = Quaternion.AngleAxis(sweepWidth * 0.5f, Vector3.up) * attackDir;

        attackDir = Quaternion.AngleAxis(-(sweepWidth * (1 - sweepCountdown * timeScalar)), Vector3.up) * attackDir;

        Vector3 origin = transform.position + attackDir * playerWidth;
        Debug.DrawLine(origin, origin + attackDir * sweepRange, Color.green);

        if(Physics.Raycast(origin, attackDir, out RaycastHit hit, sweepRange))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Destroy(hit.collider.gameObject);
                cameraShake.enableCamShake();
                stopTime.enableTimeStop();
                this.GetComponent<PlayerPoints>().AddPoints(100);
            }
        }

    }

    // fire out several short rays in front of the player to check for enemy collision
    void checkLungeCollision()
    {
        Vector3 lungePerp = Vector3.Cross(transform.up, lungeDir);
        Vector3 rayOrigin = transform.position + (lungeDir * playerWidth) + (-lungePerp * playerWidth);

        for (int i = 0; i < 5; i++)
        {
            if (Physics.Raycast(rayOrigin, lungeDir, out RaycastHit hit, lungeRange))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    Destroy(hit.collider.gameObject);
                    cameraShake.enableCamShake();
                    stopTime.enableTimeStop();
                }
            }
            rayOrigin += lungePerp * 0.2f;
            Debug.DrawLine(rayOrigin, rayOrigin + (lungeDir * lungeRange), Color.red);
        }
    }

    // activate the sweeping attack
    public void SweepAttack()
    {
        if (attackCooldown <= 0)
        {
            sweepCountdown = sweepDuration;
            attackCooldown = attackCooldownDuration;
            if(AttackPanel != null)
                AttackPanel.fillAmount = 0;
        }
    }

    // activate the lunge atack (causing the player to charge forward)
    public void LungeAttack()
    {
        if (attackCooldown <= 0)
        {
            lungeCountdown = lungeDuration;
            attackCooldown = attackCooldownDuration;
            lungeDir = transform.forward;
            if (AttackPanel != null)
                AttackPanel.fillAmount = 0;
        }
    }

    // check an area in front of the players, then return the first located player
    public GameObject GrabPlayer()
    {
        Vector3 rayOrigin = transform.position /*+ (transform.forward * playerWidth)*/;
        Vector3 rayDir = transform.forward;
        if (Physics.Raycast(rayOrigin, rayDir, out RaycastHit firstHit, 3.0f))
        {
            if (firstHit.collider.CompareTag("Player"))
            {
                return firstHit.collider.gameObject;
            }
        }

        rayDir = Quaternion.AngleAxis(-grabWidth * 0.5f, Vector3.up) * rayDir;
        
        for (int i = 0; i < 4; i++)
        {
            if (Physics.Raycast(rayOrigin, rayDir, out RaycastHit hit, 3.0f))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    Debug.Log(hit.collider.name);
                    return hit.collider.gameObject;
                }
            }
            rayDir = Quaternion.AngleAxis(grabWidth * 0.25f, Vector3.up) * rayDir;

        }
        return null;
    }

}
