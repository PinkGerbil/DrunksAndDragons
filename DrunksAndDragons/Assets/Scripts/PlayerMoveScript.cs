using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    public float speedMod = 1;

    Rigidbody rigidbody;

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

    // Start is called before the first frame update
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

    // Update is called once per frame
    void Update()
    {
        Vector3 moveDir = input.GetMoveDir;
        if (moveDir != Vector3.zero && rigidbody.isKinematic && speedMod > 0)
        {
            Vector3 aimDir = moveDir;
            float angle = Mathf.Atan2(aimDir.x, aimDir.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
            if (!CheckInDirection(transform.position + transform.forward * moveSpeed * speedMod * Time.deltaTime))
            {
                transform.position += transform.forward * moveSpeed * speedMod * Time.deltaTime;
                checkGrounded();
                isMoving = true;
            }

        }
        else if (isMoving)
        {
            isMoving = false;
        }
        animator.SetBool("Moving", isMoving);
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

        int layerMask = 1 << LayerMask.NameToLayer("Environment");
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
            if (rigidbody.isKinematic)
                transform.position = ground.point;
        }
    }

    /// <summary>
    /// Check the direction the player is moving and make sure they don't go through any walls or environment.
    /// </summary>
    /// <param name="nextPos"> the position the player will be in after force is applied </param>
    /// <returns> true if the player will collide with something </returns>
    public bool CheckInDirection(Vector3 nextPos)
    {
        Vector3 origin = TopPoint.transform.position;
        int dirInv = 1;
        Vector3 hitDir = Vector3.Normalize(nextPos - transform.position);
        //origin -= hitDir * playerRadius;

        // direction perpendicular to hitDir
        Vector3 dirPerp = Vector3.Cross(Vector3.up, hitDir);

        RaycastHit closest = new RaycastHit();

        int layerMask = 1 << LayerMask.NameToLayer("Environment");
        for (int i = 0; i < 3; i++)
        {
            if (Physics.Raycast(origin, hitDir, out RaycastHit hit, playerRadius, layerMask))
            {
                if (closest.collider == null || hit.distance < closest.distance)
                    closest = hit;
            }
            origin += dirPerp * playerRadius * dirInv;
            dirInv *= -2;
        }

        if(closest.collider != null)
        {
            Vector3 temp1 = closest.collider.ClosestPointOnBounds(nextPos);
            Vector3 temp2 = closest.collider.ClosestPointOnBounds(nextPos + (temp1 - nextPos).normalized * 100);
            Vector3 newPos = new Vector3();

            if (Vector3.Distance(closest.point, temp1) < Vector3.Distance(closest.point, temp2))
                newPos = temp1;
            else
                newPos = temp2;
            newPos += (nextPos - newPos).normalized * playerRadius;
            newPos.y = transform.position.y;
            transform.position = newPos;
            return true;
        }
        else
            return false;
    }

}
