using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerInput), typeof(PlayerMoveScript))]
public class AttackScript : MonoBehaviour
{
    [Tooltip("The distance from the player's center to the player's edge")]
    [SerializeField]
    float playerRadius = 0.5f;


    PlayerInput input;
    PlayerMoveScript playerMove;

    PlayerPoints points;

    
    [HideInInspector]
    public Image AttackPanel;

    [HideInInspector]
    public Animator animator;

    public bool IsAttacking { get { return !(sweepCountdown <= 0 && lungeCountdown <= 0); } }

    // consider having multiple attackCooldownDurations depending on what attack was used
    [Tooltip("The length of the cooldown some attacks have (currently only throw)")]
    [Range(0, 5)]
    public float attackCooldownDuration = 1.0f;
    [HideInInspector]
    public float attackCooldown = 0;

    [Header("Pickups & Produce")]
    public GameObject healthUp;
    public int healthUpPrice;
    public GameObject speedUp;
    public int speedUpPrice;
    public GameObject food;
    public int foodPrice;

    [Header("Sweep")]
    [Tooltip("How fast the sweep occurs")]
    [Range(0,5)]
    public float sweepDuration = 0.25f;
    float sweepCountdown = 0;

    [Tooltip("The range of the sweep attack")]
    [Range(0, 10)]
    public float sweepRange = 2;
    [Tooltip("How wide the sweep attack should be")]
    [Range(1, 360)]
    public float sweepWidth = 45;
    [Tooltip("how much damage the sweep does")]
    [Range(0, 6)]
    public int sweepDamage;

    [Header("Lunge")]
    [Tooltip("How long the lunge attack lasts")]
    [Range(0, 5)]
    public float lungeDuration = 0.15f;
    float lungeCountdown = 0;
    Vector3 lungeDir;


    [Tooltip("How far in front the rays of the lunge attack reach")]
    [Range(0, 5)]
    public float lungeRange = 0.5f;
    [Tooltip("how much damage the lunge does")]
    [Range(0, 6)]
    public int lungeDamage;

    [Header("Punch/Combo")]
    [Tooltip("How long the punch collision check lasts")]
    [Range(0, 2)]
    public float punchTime = 0.2f;
    [HideInInspector]
    public float punchCountdown = 0;
    [Tooltip("How much a regular punch does")]
    [Range(0, 6)]
    public int punchDamage = 1;
    [Tooltip("How far the punch reaches")]
    [Range(0, 3)]
    public float punchRange = 0.5f;
    [Tooltip("How long the player has between hits before the combo resets")]
    [Range(0, 10)]
    public float comboGracePeriod = 0.5f;
    float comboGraceCountdown = 0;
    bool punchQueued = false;

    [Header("Grab/Throw")]
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

    GameObject heldObject = null;
    public bool isHoldingObject { get { return heldObject != null; } }

    new Rigidbody rigidbody;
    

    string[] animationTriggerStrings =
    {
        "punch1",
        "punch2"
    };
    int curPunchAnim = 0;

    List<GameObject> hitEnemies = new List<GameObject>();
    static List<GameObject> heldObjects = new List<GameObject>();

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
        else
            curPunchAnim = 0;

        if(punchCountdown > 0)
        {
            punchCountdown -= Time.deltaTime;
            checkPunch();
            if (punchCountdown <= 0)
                playerMove.speedMod = 1;
            else if(punchCountdown <= punchTime * 0.5f)
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
                if (!playerMove.CheckInDirection(transform.position + lungeDir * 20 * Time.deltaTime))
                {
                    transform.position += lungeDir * 20 * Time.deltaTime;
                    playerMove.checkGrounded();
                    lungeCountdown -= Time.deltaTime;
                }
                else
                {
                    lungeCountdown = 0;
                }
                checkLungeCollision();
                if (lungeCountdown <= 0)
                {
                    hitEnemies.Clear();
                    playerMove.speedMod = 1;
                }
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
        if (rigidbody.isKinematic && !GetComponent<PlayerDamageHandler>().isKnockedBack)
        {
            if (input.GetPunchPressed && !heldObject)
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
                    GrabObject();
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
        if (hitEnemies.Count != 0) hitEnemies.Clear();
        Vector3 origin = transform.position + transform.forward * playerRadius + attackPoint;
        int layerMask = 1 << LayerMask.NameToLayer("Enemy");
        float switchdir = -1;
        float widthScale = 0.5f;
        for(int i = 0; i < 3; i++)
        {
            switchdir *= -1;
            if(Physics.Raycast(origin, transform.forward, out RaycastHit hit, punchRange, layerMask))
            {
                bool wasHit = false;

                foreach(GameObject child in hitEnemies)
                    if(hit.collider.gameObject.Equals(child))
                    {
                        wasHit = true;
                        break;
                    }
                if(!wasHit)
                {
                    if (!hit.collider.gameObject.GetComponent<AI>().channeling)
                    {
                        hit.collider.GetComponent<AI>().takeDamage(punchDamage);
                    }
                    if(hit.collider.gameObject.GetComponent<AI>().isDead)
                    {
                        points.gainKills();
                    }
                    hitEnemies.Add(hit.collider.gameObject);
                }
            }
            origin += transform.right * (playerRadius * widthScale) * switchdir;
            widthScale += 0.5f;
        }
        hitEnemies.Clear();
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
        Debug.DrawLine(origin, origin + attackDir * (sweepRange + playerRadius), Color.green);
        int layerMask = 1 << LayerMask.NameToLayer("Enemy"); ;

        if(Physics.Raycast(origin, attackDir, out RaycastHit hit, sweepRange + playerRadius, layerMask))
        {
            hit.collider.enabled = true;
            if (hit.collider.CompareTag("Enemy"))
            {
                    hit.collider.enabled = false;
                if (!hit.collider.gameObject.GetComponent<AI>().channeling)
                {
                    hit.collider.gameObject.GetComponent<AI>().takeDamage(sweepDamage);
                }
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
        Vector3 rayOrigin = transform.position + (-lungePerp * playerRadius) + attackPoint;
        int layerMask = 1 << LayerMask.NameToLayer("Enemy");
        for (int i = 0; i < 5; i++)
        {
            if (Physics.Raycast(rayOrigin, lungeDir, out RaycastHit hit, lungeRange + playerRadius, layerMask))
            {
                bool wasHit = false;
                foreach (GameObject child in hitEnemies)
                    if (hit.collider.gameObject.Equals(child))
                    {
                        wasHit = true;
                        break;
                    }
                if(!wasHit)
                {
                    hit.collider.enabled = false;
                    if (!hit.collider.gameObject.GetComponent<AI>().channeling)
                    {
                        hit.collider.gameObject.GetComponent<AI>().takeDamage(lungeDamage);
                    }
                    if (hit.collider.gameObject.GetComponent<AI>().isDead)
                        points.gainKills();
                    hit.collider.enabled = true;
                    hitEnemies.Add(hit.collider.gameObject);
                }
            }
            rayOrigin += lungePerp * 0.2f;
            Debug.DrawLine(rayOrigin, rayOrigin + (lungeDir * (lungeRange + playerRadius)), Color.red);
        }
    }
    
    public void PunchAttack()
    {
        if(punchCountdown <= punchTime * 0.25f)
        {
            punchCountdown = punchTime;
            playerMove.speedMod = 0;
            if (animator != null)
            {
                animator.SetTrigger(animationTriggerStrings[curPunchAnim++]);
                
                if (curPunchAnim == animationTriggerStrings.Length)
                    curPunchAnim = 0;
            }

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
    public void GrabObject()
    {
        Vector3 rayOrigin = transform.position + attackPoint;
        Vector3 rayDir = transform.forward;
        int layerMask = (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Pickup"));
        if (Physics.Raycast(rayOrigin, rayDir, out RaycastHit firstHit, grabRange, layerMask))
        {
            if (!(firstHit.collider.CompareTag("Player") && firstHit.collider.GetComponent<AttackScript>().isHoldingObject)) // returns false if the target is holding something/someone
            {
                bool isHeld = false;
                foreach (GameObject child in heldObjects)
                    if (firstHit.collider.gameObject.Equals(child)) // checks if the object isn't already held by someone else
                    {
                        isHeld = true;
                    }

                if (!isHeld)
                {
                    heldObject = firstHit.collider.gameObject;
                    heldObjects.Add(heldObject);
                    return;
                }
            }
        }

        rayDir = Quaternion.AngleAxis(-grabWidth * 0.5f, Vector3.up) * rayDir;
        
        for (int i = 0; i < 4; i++)
        {
            if (Physics.Raycast(rayOrigin, rayDir, out RaycastHit hit, grabRange, layerMask))
            {
                if (!(hit.collider.CompareTag("Player") && hit.collider.GetComponent<AttackScript>().isHoldingObject))
                {
                    bool isHeld = false;
                    foreach (GameObject child in heldObjects)
                        if (firstHit.collider.gameObject.Equals(child))
                        {
                            isHeld = true;
                        }
                    if (!isHeld)
                    {
                        heldObject = hit.collider.gameObject;
                        heldObjects.Add(heldObject);
                        return;
                    }
                }
            }
            rayDir = Quaternion.AngleAxis(grabWidth * 0.25f, Vector3.up) * rayDir;

        }
        return;
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
            else
            {
                heldObject.GetComponent<ThrowableObject>().wasThrown = true;
            }
            Rigidbody other = heldObject.GetComponent<Rigidbody>();
            other.isKinematic = false;
            other.AddForce((transform.forward + transform.up).normalized * throwForce);
            heldObjects.Remove(heldObject);
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
