using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using XboxCtrlrInput;

[RequireComponent(typeof(PlayerInput), typeof(Animator))]
public class PlayerMoveScript : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The PlayerInput script attached to the player object")]
    PlayerInput input;

    [SerializeField]
    [Tooltip("The AttackScript script attached to the player object")]
    AttackScript attack;

    [SerializeField] 
    [Tooltip("How fast the player moves")]
    [Range(1, 10)]
    float moveSpeed = 5;
    [Tooltip("float that increases players speed permanently")]
    public float shopSpeedIncrease;
    public float shopSpeedIncreaseLimit;

    /// <summary>
    /// A speed modifier that reduces player speed when the player is carrying another player
    /// </summary>
    public float carrySpeedMod = 1;
    /// <summary>
    /// A speed Modifier that increases the player speed after the player consumes food or drink
    /// </summary>
    public float consumableSpeedMod = 1;

    new Rigidbody rigidbody;

    [SerializeField]
    Animator animator;
    [Tooltip("Set how far in front of the player an object should be before a collision occurs")]
    [SerializeField]
    float playerRadius = 0.5f;

    bool isMoving = false;
    [Tooltip("how tall is the player")]
    [SerializeField]
    float height = 2;

    GameObject TopPoint;

    float yOffset { get { return TopPoint.transform.position.y - transform.position.y;} }

    /// <summary>
    /// Returns the position the player will be in if they walk forward this frame. (carrySpeedMod should be applied before consumableSpeedMod)
    /// </summary>
    Vector3 velocity { get { return transform.forward * (moveSpeed + shopSpeedIncrease) * carrySpeedMod * consumableSpeedMod * Time.deltaTime; } }

    
    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        if (!input)
            input = GetComponent<PlayerInput>();
        if (!attack)
            attack = GetComponent<AttackScript>();
        rigidbody = GetComponent<Rigidbody>();
        if (!animator)
            animator = GetComponent<Animator>();
        TopPoint = transform.Find("TopPoint").gameObject;
        int layerMask = 1 << 11;
        if (Physics.Raycast(TopPoint.transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, layerMask))
            height = hit.distance;
    }
    
    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        if(shopSpeedIncrease > shopSpeedIncreaseLimit)
        {
            shopSpeedIncrease = shopSpeedIncreaseLimit;
        }
        Vector3 moveDir = input.GetMoveDir;
        if (moveDir != Vector3.zero && rigidbody.isKinematic && carrySpeedMod > 0 && consumableSpeedMod > 0 && !attack.IsAttacking)
        {
            transform.rotation = Quaternion.LookRotation(moveDir, Vector3.up);
            Vector3 newVelocity = velocity;
            while (CheckInDirection(newVelocity, out Vector3 colNorm))
            {
                newVelocity = Vector3.ProjectOnPlane(newVelocity, colNorm);
            }
            transform.position += newVelocity;
            checkGrounded();
            isMoving = true;

        }
        else
        {
            isMoving = false;
        }
        animator.SetBool("Moving", isMoving);
        if(!gameObject.GetComponent<PlayerDamageHandler>().Alive && shopSpeedIncrease > 0 && gameObject.GetComponent<AttackScript>().loseSpeedUpOnDeath)
        {
            shopSpeedIncrease = 0;
        }
        
    }

    /// <summary>
    /// Fire several rays down from several points on the player to check if the
    /// ground beneath the players and see if the player should be higher or lower relative to it
    /// </summary>
    public void checkGrounded()
    {
        bool ungrounded = true;
        Vector3 origin = TopPoint.transform.position + (-transform.right * playerRadius) + (transform.forward * playerRadius);
        Vector3 highest = Vector3.zero;

        int layerMask = 1 << LayerMask.NameToLayer("Floor");
        for (int i = 0; i < 3; i++)
        {
            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                Vector3 temp = hit.point;
                temp.y = TopPoint.transform.position.y;
                float distance = Vector3.Distance(temp, hit.point);
                if (distance <= height)
                {
                    Vector3 current = Vector3.up * (height - distance);
                    if (current.y > highest.y)
                        highest = current;
                    ungrounded = false;
                }
            }
            origin += transform.right * playerRadius;
        }
        if(highest != Vector3.zero)
        {
            transform.position += highest;
            return;
        }
        origin = transform.position + ((TopPoint.transform.position - transform.position).normalized * 0.75f) + (-transform.right * playerRadius) + (-transform.forward * playerRadius);
        for (int i = 0; i < 3; i++)
        {
            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                Vector3 temp = TopPoint.transform.position;
                temp.y = hit.point.y;
                float distance = Vector3.Distance(temp, TopPoint.transform.position);
                if (distance <= height)
                {
                    ungrounded = false;
                }
            }
            origin += transform.right * playerRadius;
        }

        if (ungrounded && Physics.Raycast(TopPoint.transform.position, Vector3.down, out RaycastHit ground, Mathf.Infinity, layerMask))
        {
            Debug.DrawLine(TopPoint.transform.position, TopPoint.transform.position + Vector3.down * (yOffset * 2));
            if (rigidbody.isKinematic)
                transform.position = ground.point;
        }
    }

    /// <summary>
    /// Checks the direction the player is moving in a returns a colision normal
    /// </summary>
    /// <param name="curVelocity">  </param>
    /// <returns></returns>
    public bool CheckInDirection(Vector3 curVelocity, out Vector3 colNorm)
    {
        Vector3 origin = transform.position;
        Vector3 hitDir = curVelocity.normalized;

        // direction perpendicular to hitDir
        Vector3 dirPerp = Vector3.Cross(Vector3.up, hitDir);

        RaycastHit closest = new RaycastHit(); // this variable will be used to store the closest RaycastHit from the following loop

        int originDirOffset = 1; // used to control the direction of the origin offset occurring at the end of each for loop iteration
        int layerMask = (1 << LayerMask.NameToLayer("Environment")) | (1 << LayerMask.NameToLayer("Pickup"));
        for (int j = 0; j < 4; j++)
        {
            for (int i = 0; i < 3; i++)
            {
                Debug.DrawRay(origin, hitDir);
                if (Physics.Raycast(origin, hitDir, out RaycastHit hit, playerRadius, layerMask))
                {
                    if (closest.collider == null || hit.distance < closest.distance)
                        closest = hit;
                }
                origin += dirPerp * playerRadius * originDirOffset;
                originDirOffset *= -2;
            }
            origin = transform.position + Vector3.up * height * (0.5f * (j + 1));
            originDirOffset = 1;
        }

        if (closest.collider != null)
        {
            colNorm = closest.normal;
            if (closest.collider.gameObject.layer == LayerMask.NameToLayer("Pickup"))
            {
                colNorm.y = curVelocity.normalized.y;
                colNorm.Normalize();
                Debug.Log(colNorm);
            }
            return true;
        }
        else
        {
            colNorm = Vector3.zero;
            return false;
        }
    }

    public bool CheckInDirection(Vector3 curVelocity, out Vector3 colNorm, out string tag)
    {
        Vector3 origin = transform.position;
        Vector3 hitDir = curVelocity.normalized;

        // direction perpendicular to hitDir
        Vector3 dirPerp = Vector3.Cross(Vector3.up, hitDir);

        RaycastHit closest = new RaycastHit(); // this variable will be used to store the closest RaycastHit from the following loop

        int originDirOffset = 1; // used to control the direction of the origin offset occurring at the end of each for loop iteration
        int layerMask = (1 << LayerMask.NameToLayer("Environment")) | (1 << LayerMask.NameToLayer("Pickup"));
        for (int j = 0; j < 4; j++)
        {
            for (int i = 0; i < 3; i++)
            {
                Debug.DrawRay(origin, hitDir);
                if (Physics.Raycast(origin, hitDir, out RaycastHit hit, playerRadius, layerMask))
                {
                    if (closest.collider == null || hit.distance < closest.distance)
                        closest = hit;
                }
                origin += dirPerp * playerRadius * originDirOffset;
                originDirOffset *= -2;
            }
            origin = transform.position + Vector3.up * height * (0.5f * (j + 1));
            originDirOffset = 1;
        }

        if (closest.collider != null)
        {
            colNorm = closest.normal;
            tag = closest.collider.tag;
            return true;
        }
        else
        {
            tag = "untagged";
            colNorm = Vector3.zero;
            return false;
        }
    }

}
