using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(timeStop), typeof(PlayerInput), typeof(PlayerMoveScript))]
public class AttackScript : MonoBehaviour
{
    [SerializeField]
    float playerWidth = 0.5f;
    [SerializeField]
    float playerHeight = 2;

    [SerializeField] cameraShake cameraShake;
    [SerializeField] timeStop stopTime;

    [SerializeField] PlayerInput input;
    [SerializeField] PlayerMoveScript playerMove;

    [SerializeField]
    [Tooltip("AttackPanel should be a panel in the UI with a horizontal fill method")]
    public Image AttackPanel;

    [SerializeField]
    public Animator animator;

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
    
    public GameObject heldObject = null;

    Rigidbody rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        if (!animator)
            animator = GetComponent<Animator>();
        if (!input)
            input = GetComponent<PlayerInput>();
        if (!playerMove)
            playerMove = GetComponent<PlayerMoveScript>();
        if (!stopTime)
            stopTime = GetComponent<timeStop>();
        if (!cameraShake)
            cameraShake = Camera.main.GetComponent<cameraShake>();

        rigidbody = GetComponent<Rigidbody>();

        if (AttackPanel != null)
            AttackPanel.transform.parent.gameObject.SetActive(gameObject.activeInHierarchy);
        
        
    }

    // Update is called once per frame
    void Update()
    {


        if (sweepCountdown > 0)
        {
            checkSweepCollide();
            sweepCountdown -= Time.deltaTime;
        }
        else if(lungeCountdown > 0)
        {
            if (lungeCountdown > lungeDuration)
                lungeCountdown -= Time.deltaTime;
            else
            {
                transform.position += lungeDir * 20 * Time.deltaTime;
                checkLungeCollision();
                lungeCountdown -= Time.deltaTime;
                playerMove.speedMod = 1;
            }
            playerMove.checkGrounded();
        }
        else if(attackCooldown > 0 && !heldObject)
        {
            attackCooldown -= Time.deltaTime;
            setCooldownGauge();
        }

        if (rigidbody.isKinematic && Time.timeScale > 0)
        {
            if (input.GetSweepPressed && !heldObject)
            {
                SweepAttack();
            }

            if (input.GetLungePressed && !heldObject)
            {
                
                LungeAttack();
            }

            if (input.GetGrabPressed)
            {
                if (!heldObject)
                {
                    heldObject = GrabObject();
                    if (heldObject != null)
                    {
                        playerMove.speedMod = 0.25f;
                        if(!heldObject.CompareTag("Player"))
                            heldObject.GetComponent<Rigidbody>().isKinematic = true;
                    }
                }
                else
                {
                    dropHeldObject();
                }

            }
        }
        else
            dropHeldObject();

        if (heldObject != null)
        {
            attackCooldown += 0.5f * Time.deltaTime;
            setCooldownGauge();
            float objectHeight;
            if (heldObject.CompareTag("Player"))
                objectHeight = 0;
            else
                objectHeight = heldObject.transform.localScale.y * 0.5f;
            heldObject.transform.SetPositionAndRotation(transform.position + (transform.up * (playerHeight + objectHeight) ), transform.rotation);

        }

        if (attackCooldown >= 1 && heldObject != null)
            dropHeldObject();
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

        Vector3 origin = transform.position + transform.up * 0.25f;
        Debug.DrawLine(origin, origin + attackDir * (sweepRange + playerWidth), Color.green);
        int layerMask = 1 << 9;

        if(Physics.Raycast(origin, attackDir, out RaycastHit hit, sweepRange + playerWidth, layerMask))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                hit.collider.enabled = false;
                hit.collider.gameObject.GetComponent<AI>().isDead = true;
                GetComponent<PlayerPoints>().AddPoints(100);
                cameraShake.enableCamShake();
                stopTime.enableTimeStop();
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
        Vector3 rayOrigin = transform.position + (-lungePerp * playerWidth) + (Vector3.up * 0.25f);
        int layerMask = 1 << 9;
        for (int i = 0; i < 5; i++)
        {
            if (Physics.Raycast(rayOrigin, lungeDir, out RaycastHit hit, lungeRange + playerWidth, layerMask))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    hit.collider.enabled = false;
                    hit.collider.gameObject.GetComponent<AI>().isDead = true;
                    GetComponent<PlayerPoints>().AddPoints(100);
                    cameraShake.enableCamShake();
                    stopTime.enableTimeStop();
                }
            }
            rayOrigin += lungePerp * 0.2f;
            Debug.DrawLine(rayOrigin, rayOrigin + (lungeDir * (lungeRange + playerWidth)), Color.red);
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
            if(animator != null)
                animator.SetTrigger("Swiping");
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
            if(animator != null)
                animator.SetTrigger("Lunging");
            lungeCountdown = lungeDuration * 4.5f;
            attackCooldown = attackCooldownDuration;
            lungeDir = transform.forward;

            playerMove.speedMod = 0;
            if (AttackPanel != null)
                AttackPanel.fillAmount = 0;
        }
    }

    /// <summary>
    /// check an area in front of the players, then return the first located player
    /// </summary>
    /// <returns> The first player hit by a ray </returns>
    public GameObject GrabObject()
    {
        Vector3 rayOrigin = transform.position;
        Vector3 rayDir = transform.forward;
        int layerMask = (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Pickup"));
        if (Physics.Raycast(rayOrigin, rayDir, out RaycastHit firstHit, grabRange, layerMask))
        {
            if (!(firstHit.collider.CompareTag("Player") && firstHit.collider.GetComponent<AttackScript>().heldObject))
            {
                return firstHit.collider.gameObject;
            }
        }

        rayDir = Quaternion.AngleAxis(-grabWidth * 0.5f, Vector3.up) * rayDir;
        
        for (int i = 0; i < 4; i++)
        {
            if (Physics.Raycast(rayOrigin, rayDir, out RaycastHit hit, grabRange, layerMask))
            {
                if (!(firstHit.collider.CompareTag("Player") && firstHit.collider.GetComponent<AttackScript>().heldObject))
                {

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
    public void dropHeldObject()
    {
        if (heldObject != null)
        {
            if (heldObject.CompareTag("Player"))
            {
                heldObject.GetComponent<Animator>().SetTrigger("Thrown");
                heldObject.GetComponent<Collider>().isTrigger = true;
            }
            Rigidbody other = heldObject.GetComponent<Rigidbody>();
            other.isKinematic = false;
            other.AddForce((transform.forward + transform.up).normalized * 500.0f);
            heldObject = null;
            playerMove.speedMod = 1;
        }
    }

    void setCooldownGauge()
    {
        if (AttackPanel != null)
            AttackPanel.fillAmount = 1 - ((1 / attackCooldownDuration) * attackCooldown);
    }
}
