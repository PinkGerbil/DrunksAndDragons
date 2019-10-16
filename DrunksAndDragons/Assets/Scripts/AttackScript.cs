using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(PlayerInput), typeof(PlayerMoveScript))]
public class AttackScript : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Plays audio whenever the player lands a hit on an enemy")]
    UnityEvent OnHit;
    [SerializeField]
    [Tooltip("Plays audio whenever player starts a punch attack")]
    UnityEvent OnAttack;

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

    public bool IsAttacking { get { return !(sweepCountdown <= 0 && lungeCountdown <= 0 && punchCountdown <= 0 && dodgeCountdown <= 0); } }

    
    [Tooltip("How Long the player can carry objects or other players for")]
    [Range(0, 5)]
    public float MaxCarryStamina = 1.0f;
    [HideInInspector]
    public float carryStamina = 0;

    [Header("shop Pickups & Produce")]
    public int fullHealPrice;
    public bool loseHealthUpOnDeath;
    public int healthUpPrice;
    public bool loseSpeedUpOnDeath;
    public int speedUpPrice;
    public int livesUpPrice;

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
    [Range(0, 6)]
    public float punchRange = 3.0f;
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


    [Header("Dodge")]
    [SerializeField]
    [Tooltip("How fast dodge moves")]
    float dodgeSpeed = 25;
    [SerializeField]
    [Tooltip("How long dodge lasts")]
    float dodgeTime = 0.25f;
    float dodgeCountdown = 0;
    [SerializeField]
    [Tooltip("How long the player has to wait to dodge again")]
    float dodgeCooldown = 0.25f;
    float dodgeCooldownCountdown = 0;

    bool isDodging { get { return dodgeCountdown > 0; } }

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
                playerMove.carrySpeedMod = 1;
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
                Vector3 velocity = lungeDir * 20 * Time.deltaTime;
                while (playerMove.CheckInDirection(velocity, out Vector3 colNorm))
                {
                    velocity = Vector3.ProjectOnPlane(velocity, colNorm);
                }
                transform.position += velocity;
                playerMove.checkGrounded();
                checkLungeCollision();
                lungeCountdown -= Time.deltaTime;
                if (lungeCountdown <= 0)
                {
                    hitEnemies.Clear();
                    playerMove.carrySpeedMod = 1;
                }
            }
        }
        else if(dodgeCountdown > 0)
        {
            dodgeCountdown -= Time.deltaTime;
            Vector3 velocity = transform.forward * dodgeSpeed * Time.deltaTime;
            while (playerMove.CheckInDirection(velocity, out Vector3 colNorm))
                velocity = Vector3.ProjectOnPlane(velocity, colNorm);
            transform.position += velocity;
            playerMove.checkGrounded();
            if (dodgeCountdown <= 0)
                dodgeCooldownCountdown = dodgeCooldown;
        }
        
        if(dodgeCooldownCountdown > 0)
        {
            dodgeCooldownCountdown -= Time.deltaTime;
        }

        if (heldObject != null)
        {

            if (heldObject.CompareTag("Player"))
            {
                carryStamina -= Time.deltaTime;
                heldObject.transform.SetPositionAndRotation(transform.Find("TopPoint").position, transform.Find("TopPoint").rotation);
            }
            else
            {
                float objectHeight;
                objectHeight = heldObject.transform.localScale.y * 0.5f;
                heldObject.transform.SetPositionAndRotation(transform.Find("TopPoint").position + new Vector3(0, objectHeight, 0), transform.Find("TopPoint").rotation);
            }

        }
        if (rigidbody.isKinematic && !GetComponent<PlayerDamageHandler>().isKnockedBack)
        {
            if (input.GetPunchPressed && !heldObject)
            {
                PunchAttack();
                
            }
            else if (input.GetLungePressed && !heldObject)
            {

                LungeAttack();
            }
            else if (input.GetGrabPressed)
            {
                if (!heldObject)
                {
                    GrabObject();
                    if (heldObject != null)
                    {
                        carryStamina = MaxCarryStamina;
                        if (heldObject.CompareTag("Player"))
                            playerMove.carrySpeedMod = 0.25f;
                        else
                            heldObject.GetComponent<Rigidbody>().isKinematic = true;
                    }
                }
                else
                {
                    dropHeldObject();
                }

            }
            if (input.GetDodgePressed)
                dodgeRoll();
        }


        if(!gameObject.GetComponent<PlayerDamageHandler>().Alive && heldObject != null)
        {
            dropHeldObject();
        }

        if (carryStamina <= 0 && heldObject != null)
            dropHeldObject();
    }

    void dodgeRoll()
    {
        if (dodgeCountdown <= 0 && dodgeCooldownCountdown <= 0)
        {
            dodgeCountdown = dodgeTime;
            animator.SetTrigger("Dodge");
        }
    }

    void checkPunch()
    {
        if (hitEnemies.Count != 0) hitEnemies.Clear();
        Vector3 origin = transform.position - (transform.forward * playerRadius) + attackPoint;
        int layerMask = 1 << LayerMask.NameToLayer("Enemy");
        float switchdir = -1;
        float widthScale = 0.5f;
        bool hitCollided = false;
        for (int i = 0; i < 3; i++)
        {
            switchdir *= -1;
            if (Physics.Raycast(origin, transform.forward, out RaycastHit hit, punchRange, layerMask))
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
                    hitCollided = true;
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
        if (hitCollided)
            OnHit.Invoke();
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
        Vector3 rayOrigin = transform.position + (-lungePerp * playerRadius) + (-transform.forward * playerRadius)+ attackPoint;
        int layerMask = 1 << LayerMask.NameToLayer("Enemy");
        bool hitCollided = false;
        for (int i = 0; i < 5; i++)
        {
            if (Physics.Raycast(rayOrigin, lungeDir, out RaycastHit hit, lungeRange + playerRadius * 2, layerMask))
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
                    hitCollided = true;
                    hit.collider.enabled = false;
                    if (!hit.collider.gameObject.GetComponent<AI>().channeling)
                    {
                        hit.collider.gameObject.GetComponent<AI>().takeDamage(lungeDamage, lungeDir);
                    }
                    if (hit.collider.gameObject.GetComponent<AI>().isDead)
                        points.gainKills();
                    hit.collider.enabled = true;
                    hitEnemies.Add(hit.collider.gameObject);
                }
            }
            rayOrigin += lungePerp * 0.2f;
            Debug.DrawLine(rayOrigin, rayOrigin + (lungeDir * (lungeRange + playerRadius * 2)), Color.red);
        }
        if (hitCollided)
        {
            OnHit.Invoke();
        }
    }
    
    public void PunchAttack()
    {
        if(punchCountdown <= punchTime * 0.25f)
        {
            OnAttack.Invoke();
            punchCountdown = punchTime;
            playerMove.carrySpeedMod = 0;
            if (animator != null)
            {
                animator.SetTrigger(animationTriggerStrings[curPunchAnim++]);
                
                if (curPunchAnim == animationTriggerStrings.Length)
                    curPunchAnim = 0;
            }

        }
    }
    

    /// <summary>
    /// Start the lunge attack by starting the lungeCountdown and attackCooldown
    /// </summary>
    public void LungeAttack()
    {
        if (lungeCountdown <= 0)
        {
            OnAttack.Invoke();
            if(animator != null)
                animator.SetTrigger("Lunging");
            lungeCountdown = lungeDuration * 4.5f;
            lungeDir = transform.forward;

            playerMove.carrySpeedMod = 0;
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
                    if(!firstHit.collider.CompareTag("Environment"))
                    {
                        heldObject = firstHit.collider.gameObject;
                        heldObjects.Add(heldObject);
                    }
                    else
                    {
                        firstHit.collider.GetComponent<TableFlip>().flip(transform.position);
                    }
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
                        if (hit.collider.gameObject.Equals(child))
                        {
                            isHeld = true;
                        }
                    if (!isHeld)
                    {
                        if (!hit.collider.CompareTag("Environment"))
                        {
                            heldObject = hit.collider.gameObject;
                            heldObjects.Add(heldObject);
                        }
                        else
                        {
                            hit.collider.GetComponent<TableFlip>().flip(transform.position);
                        }
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
                heldObject.GetComponent<ThrowableObject>().setThrown();
            }
            Rigidbody other = heldObject.GetComponent<Rigidbody>();
            other.isKinematic = false;
            other.AddForceAtPosition((transform.forward + transform.up).normalized * throwForce, other.ClosestPointOnBounds(other.transform.position - transform.forward * playerRadius));
            heldObjects.Remove(heldObject);
            heldObject = null;
            playerMove.carrySpeedMod = 1;
        }
    }
    

    //buying stuff in the bar
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "FullHealShop" && input.getBuyPressed && points.points >= fullHealPrice)
        {
            gameObject.GetComponent<PlayerDamageHandler>().health = gameObject.GetComponent<PlayerDamageHandler>().maxHealth;
            points.LosePoints(fullHealPrice);
        }
        else if (other.gameObject.tag == "SpeedDrinkShop" && input.getBuyPressed && points.points >= speedUpPrice)
        {
            this.gameObject.GetComponent<PlayerMoveScript>().shopSpeedIncrease++;
            points.LosePoints(speedUpPrice);
        }
        else if (other.gameObject.tag == "HealthIncreaseShop" && input.getBuyPressed && points.points >= healthUpPrice)
        {
            gameObject.GetComponent<PlayerDamageHandler>().maxHealth++;
            gameObject.GetComponent<PlayerDamageHandler>().health++;
            points.LosePoints(healthUpPrice);
        }
        else if (other.gameObject.tag == "LivesUpShop" && input.getBuyPressed && points.points >= livesUpPrice)
        {
            gameObject.GetComponent<PlayerDamageHandler>().lives++;
            points.LosePoints(livesUpPrice);
        }
    }

}
