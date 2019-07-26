using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AttackScript), typeof(PlayerInput))]
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

    float speedMod = 1;

    GameObject heldPlayer = null;


    // Start is called before the first frame update
    void Start()
    {
        if (!input)
            input = GetComponent<PlayerInput>();
        if (!attack)
            attack = GetComponent<AttackScript>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 aimDir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        float angle = Mathf.Atan2(aimDir.x, aimDir.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);

        if (input.GetSweepPressed && !heldPlayer)
        {
            attack.SweepAttack();
        }

        if(input.GetLungePressed && !heldPlayer)
        {
            attack.LungeAttack();
        }

        if(input.GetGrabPressed)
        {
            if (!heldPlayer)
            {
                heldPlayer = attack.GrabPlayer();
                if(heldPlayer != null)
                    speedMod = 0.25f;
            }
            else
            {
                heldPlayer.GetComponent<Rigidbody>().isKinematic = false;
                heldPlayer.GetComponent<Rigidbody>().AddForceAtPosition((transform.forward + transform.up).normalized * 500.0f, heldPlayer.transform.position - transform.forward * 0.5f);
                heldPlayer = null;
                speedMod = 1;
            }

        }

        if (!!heldPlayer)
            heldPlayer.transform.SetPositionAndRotation(transform.position + (transform.up * 2), transform.rotation);

        transform.position += input.GetMoveDir * moveSpeed * speedMod * Time.deltaTime;
        
    }
}
