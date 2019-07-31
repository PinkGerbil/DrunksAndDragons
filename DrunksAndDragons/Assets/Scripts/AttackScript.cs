using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(timeStop), typeof(PlayerInput), typeof(PlayerMoveScript))]
public class AttackScript : MonoBehaviour
{
    const float playerWidth = 0.5f;

    [SerializeField] cameraShake cameraShake;
    [SerializeField] timeStop stopTime;

    [SerializeField] PlayerInput input;
    [SerializeField] PlayerMoveScript playerMove;

    [SerializeField]
    [Tooltip("AttackPanel should be a panel in the UI with a horizontal fill method")]
    public Image AttackPanel;

    public bool IsAttacking { get { return !(sweepCountdown <= 0 && lungeCountdown <= 0); } }

    // consider having multiple attackCooldownDurations depending on what attack was used
    [Range(0, 5)]
    public float attackCooldownDuration = 1.0f;
    [HideInInspector]
    public float attackCooldown = 0;

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

    [HideInInspector]
    public GameObject heldPlayer = null;

    // Start is called before the first frame update
    void Start()
    {
        if (!input)
            input = GetComponent<PlayerInput>();
        if (!playerMove)
            playerMove = GetComponent<PlayerMoveScript>();
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
        else if(attackCooldown > 0 && !heldPlayer)
        {
            attackCooldown -= Time.deltaTime;
            setCooldownGauge();
        }

        if (input.GetSweepPressed && !heldPlayer)
        {
            SweepAttack();
        }

        if (input.GetLungePressed && !heldPlayer)
        {
            LungeAttack();
        }

        if (input.GetGrabPressed)
        {
            if (!heldPlayer)
            {
                heldPlayer = GrabPlayer();
                if (heldPlayer != null)
                    playerMove.speedMod = 0.25f;
            }
            else
            {
                dropPlayer();
            }

        }

        if (!!heldPlayer)
        {
            attackCooldown += Time.deltaTime;
            setCooldownGauge();
            heldPlayer.transform.SetPositionAndRotation(transform.position + (transform.up * 2), transform.rotation);
        }

        if (attackCooldown >= 1 && heldPlayer != null)
            dropPlayer();
    }

    /// <summary>
    /// Fire out one ray each frame. The direction of the frame relative to the player depends on the sweepCountdown
    /// </summary>
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

    /// <summary>
    /// Fire out five parallel rays in front of the player to check for enemies.
    /// If an enemy is hit, they are destroyed.
    /// </summary>
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

    /// <summary>
    /// Start the sweep attack by starting the sweepCountdown and attackCooldown
    /// </summary>
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

    /// <summary>
    /// Start the lunge attack by starting the lungeCountdown and attackCooldown
    /// </summary>
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

    /// <summary>
    /// check an area in front of the players, then return the first located player
    /// </summary>
    /// <returns> The first player hit by a ray </returns>
    public GameObject GrabPlayer()
    {
        Vector3 rayOrigin = transform.position;
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

    /// <summary>
    /// Throw held Player and allow them to move again
    /// </summary>
    public void dropPlayer()
    {
        heldPlayer.GetComponent<Rigidbody>().isKinematic = false;
        heldPlayer.GetComponent<Rigidbody>().AddForceAtPosition((transform.forward + transform.up).normalized * 500.0f, heldPlayer.transform.position - transform.forward * 0.5f);
        heldPlayer = null;
        playerMove.speedMod = 1;
    }

    void setCooldownGauge()
    {

        if (AttackPanel != null)
            AttackPanel.fillAmount = 1 - ((1 / attackCooldownDuration) * attackCooldown);
    }
}
