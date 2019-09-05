using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerInput), typeof(PlayerMoveScript))]
public class AttackScript : MonoBehaviour
{
    [SerializeField]
    float playerWidth = 0.5f;
    [SerializeField]
    float playerHeight = 2;

    [SerializeField] PlayerInput input;
    [SerializeField] PlayerMoveScript playerMove;

    [SerializeField] PlayerPoints points;

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
    [Tooltip("how much damage the sweep does")]
    [Range(0, 6)]
    public int sweepDamage;

    [Tooltip("How far in front the rays of the lunge attack reach")]
    [Range(0, 5)]
    public float lungeRange = 0.5f;
    [Tooltip("how much damage the lunge does")]
    [Range(0, 6)]
    public int lungeDamage;

    [Range(0, 2)]
    public float punchTime = 0.2f;
    [HideInInspector]
    public float punchCountdown = 0;
    [Range(0, 2)]
    public float comboGracePeriod = 0.5f;
    float comboGraceCountdown = 0;
    [Range(0, 6)]
    public int punchDamage = 1;
    [Range(0, 3)]
    public float punchRange = 0.5f;

    [Tooltip("How far away the player can grab another player from")]
    [Range(0, 5)]
    public float grabRange = 1.5f;
    [Tooltip("The angle of the grab check in front of the player")]
    [Range(1, 360)]
    public float grabWidth = 45.0f;

    [Tooltip("The force at which the player throws held items/players")]
    [Range(0, 1000)]
    public float throwForce = 500.0f;

    Vector3 attackPoint { get { return (transform.Find("TopPoint").position - transform.position) * 0.25f; } }

    public GameObject heldObject = null;

    Rigidbody rigidbody;
    
    public GameObject healthUp;
    public int healthUpPrice;
    public GameObject speedUp;
    public int speedUpPrice;
    public GameObject food;
    public int foodPrice;


    // Start is called before the first frame update
    void Start()
    {
        
        if (!animator)
            animator = GetComponent<Animator>();
        if (!input)
            input = GetComponent<PlayerInput>();
        if (!playerMove)
            playerMove = GetComponent<PlayerMoveScript>();
        if (!points)
            points = GetComponent<PlayerPoints>();

        rigidbody = GetComponent<Rigidbody>();

        if (AttackPanel != null)
            AttackPanel.transform.parent.gameObject.SetActive(gameObject.activeInHierarchy);
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (comboGraceCountdown > 0)
            comboGraceCountdown -= Time.deltaTime;

        if(punchCountdown > 0)
        {
            punchCountdown -= Time.deltaTime;
            checkPunch();
            if (punchCountdown > 0)
                comboGraceCountdown = comboGracePeriod;
        }
        else if (sweepCountdown > 0)
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
                playerMove.checkGrounded();
                checkLungeCollision();
                lungeCountdown -= Time.deltaTime;
                playerMove.speedMod = 1;
            }
        }
        else if(attackCooldown > 0 && !heldObject)
        {
            attackCooldown -= Time.deltaTime;
            setCooldownGauge();
        }

        if (heldObject != null)
        {
            attackCooldown += 0.5f * Time.deltaTime;
            setCooldownGauge();
            float objectHeight;
            if (heldObject.CompareTag("Player"))
                objectHeight = 0;
            else
                objectHeight = heldObject.transform.localScale.y * 0.5f;
            heldObject.transform.SetPositionAndRotation(transform.Find("TopPoint").position + new Vector3(0, objectHeight, 0), transform.Find("TopPoint").rotation);

        }
        if (rigidbody.isKinematic && Time.timeScale > 0)
        {
            if (input.GetSweepPressed && !heldObject)
            {
                //SweepAttack();
                PunchAttack();
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




        if (attackCooldown >= 1 && heldObject != null)
            dropHeldObject();
    }

    void checkPunch()
    {
        Vector3 origin = transform.position + transform.forward * playerWidth + attackPoint;
        int layerMask = 1 << LayerMask.NameToLayer("Enemy");

        for(int i = 0; i < 3; i++)
        {
            if(Physics.Raycast(origin, transform.forward, out RaycastHit hit, punchRange, layerMask))
            {
                Debug.DrawLine(origin, origin + transform.forward * punchRange, Color.green);
                hit.collider.GetComponent<AI>().takeDamage(punchDamage);
                if (hit.collider.gameObject.GetComponent<AI>().isDead)
                {
                    points.gainKills();
                }
            }
            origin += transform.right * (playerWidth * 0.5f);
        }
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

        Vector3 origin = transform.position + attackPoint;
        Debug.DrawLine(origin, origin + attackDir * (sweepRange + playerWidth), Color.green);
        int layerMask = 1 << LayerMask.NameToLayer("Enemy"); ;

        if(Physics.Raycast(origin, attackDir, out RaycastHit hit, sweepRange + playerWidth, layerMask))
        {
            hit.collider.enabled = true;
            if (hit.collider.CompareTag("Enemy"))
            {
                    hit.collider.enabled = false;
                    hit.collider.gameObject.GetComponent<AI>().takeDamage(sweepDamage);
                    if (hit.collider.gameObject.GetComponent<AI>().isDead)
                    {
                        points.gainKills();
                    }
                    //hit.collider.gameObject.GetComponent<AI>().timeAIInvulnurable++;
            }
            hit.collider.enabled = true;

        }

    }

    /// <summary>
    /// Fire out five parallel rays in front of the player to check for enemies.
    /// If an enemy is hit, they are destroyed.
    /// </summary>
    void checkLungeCollision()
    {
        Vector3 lungePerp = Vector3.Cross(transform.up, lungeDir);
        Vector3 rayOrigin = transform.position + (-lungePerp * playerWidth) + attackPoint;
        int layerMask = 1 << LayerMask.NameToLayer("Enemy");
        for (int i = 0; i < 5; i++)
        {
            if (Physics.Raycast(rayOrigin, lungeDir, out RaycastHit hit, lungeRange + playerWidth, layerMask))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                        hit.collider.enabled = false;
                        hit.collider.gameObject.GetComponent<AI>().takeDamage(lungeDamage);
                        if (hit.collider.gameObject.GetComponent<AI>().isDead)
                        {
                            points.gainKills();
                        }
                }
                hit.collider.enabled = true;
            }
            rayOrigin += lungePerp * 0.2f;
            Debug.DrawLine(rayOrigin, rayOrigin + (lungeDir * (lungeRange + playerWidth)), Color.red);
        }
    }
    
    public void PunchAttack()
    {
        if(punchCountdown <= 0)
        {
            punchCountdown = punchTime;
            if (animator != null)
                if (comboGraceCountdown > 0)
                    Debug.Log("No alt-punch animation implemented");
                else
                    Debug.Log("No punch animation implemented");

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
        Vector3 rayOrigin = transform.position + attackPoint;
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
                if (!(hit.collider.CompareTag("Player") && hit.collider.GetComponent<AttackScript>().heldObject))
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
            Debug.Log(other.isKinematic);
            other.AddForce((transform.forward + transform.up).normalized * throwForce);
            
            heldObject = null;
            playerMove.speedMod = 1;
        }
    }

    void setCooldownGauge()
    {
        if (AttackPanel != null)
            AttackPanel.fillAmount = 1 - ((1 / attackCooldownDuration) * attackCooldown);
    }

    //buying stuff in the bar
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "FoodShop" && input.getBuyPressed && points.points >= foodPrice)
        {
            Instantiate(food,this.transform.position,this.transform.rotation);
            points.LosePoints(foodPrice);
        }
        else if (other.gameObject.tag == "SpeedDrinkShop" && input.getBuyPressed && points.points >= speedUpPrice)
        {
            Instantiate(speedUp, this.transform.position, this.transform.rotation);
            points.LosePoints(speedUpPrice);
        }
        else if (other.gameObject.tag == "HealthIncreaseShop" && input.getBuyPressed && points.points >= healthUpPrice)
        {
            Instantiate(healthUp, this.transform.position, this.transform.rotation);
            points.LosePoints(healthUpPrice);
        }
    }
}
