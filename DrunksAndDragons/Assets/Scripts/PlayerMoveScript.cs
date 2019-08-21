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
    [Range(1, 10)]
    float moveSpeed = 5;

    public float speedMod = 1;

    Rigidbody rigidbody;

    [SerializeField]
    Animator animator;
    [Tooltip("E.G. set how far in front of the player an object should be before a collision occurs")]
    [SerializeField]
    float playerRadius = 0.5f;

    bool isMoving = false;
    [Tooltip("how tall is the player")]
    [SerializeField]
    float height = 2;

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
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveDir = input.GetMoveDir;
        if (moveDir != Vector3.zero && rigidbody.isKinematic && Time.timeScale > 0 && speedMod > 0)
        {
            transform.position += moveDir * moveSpeed * speedMod * Time.deltaTime;
            Vector3 aimDir = moveDir;
            float angle = Mathf.Atan2(aimDir.x, aimDir.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
            
            isMoving = true;

            checkGrounded();
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
        bool grounded = true;
        Vector3 origin = transform.position + (-transform.right * playerRadius) + (transform.forward * playerRadius);
        Vector3 highest = Vector3.zero;
        for (int i = 0; i < 3; i++)
        {
            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit))
            {
                Vector3 temp = hit.point;
                temp.y = transform.position.y;
                float distance = Vector3.Distance(temp, hit.point);
                if (hit.collider.CompareTag("Environment") && distance <= height * 0.5f)
                {
                    Vector3 current = Vector3.up * (height * 0.5f - distance);
                    if (current.magnitude > highest.magnitude)
                    highest = Vector3.up * (height * 0.5f - distance);
                    grounded = false;
                }
            }
            origin += transform.right * playerRadius;
        }
        if(highest != Vector3.zero)
        {
            transform.position += highest;
            return;
        }
        origin = transform.position + (-transform.right * playerRadius) + (-transform.forward * playerRadius);
        for (int i = 0; i < 3; i++)
        {
            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit))
            {
                Vector3 temp = hit.point;
                temp.y = transform.position.y;
                float distance = Vector3.Distance(temp, hit.point);
                if (hit.collider.CompareTag("Environment") && distance <= height * 0.5f)
                {
                    grounded = false;
                }
            }
            origin += transform.right * playerRadius;
        }

        if (grounded && Physics.Raycast(transform.position, Vector3.down, out RaycastHit ground))
        {
            transform.position = ground.point + (Vector3.up * (height * 0.5f));
        }
    }

}
