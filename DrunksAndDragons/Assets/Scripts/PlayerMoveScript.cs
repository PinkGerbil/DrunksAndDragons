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

    [SerializeField]
    [Range(2, 5)]
    float boostSpeed = 2;
    float speedMod = 1;

    [SerializeField]
    [Range(0, 5)]
    float BoostTime = 1;
    float boostTimer = 0;

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

        if (input.GetSweepPressed)
        {
            attack.SweepAttack();
        }

        if(input.GetLungePressed)
        {
            attack.LungeAttack();
        }

        if(input.GetGrabPressed)
        {
            if (!heldPlayer)
            {
                heldPlayer = attack.GrabPlayer();
                Debug.Log(heldPlayer);
            }
            else
            {
                heldPlayer.GetComponent<Rigidbody>().isKinematic = false;
                heldPlayer.GetComponent<Rigidbody>().AddForceAtPosition((transform.forward + transform.up).normalized * 500.0f, heldPlayer.transform.position - transform.forward * 0.5f);
                heldPlayer = null;
            }

        }

        if (!!heldPlayer)
            heldPlayer.transform.SetPositionAndRotation(transform.position + (transform.up * 2), transform.rotation);

        transform.position += input.GetMoveDir * (moveSpeed * speedMod) * Time.deltaTime;

        if (boostTimer > 0)
            boostTimer -= Time.deltaTime;
        else
            speedMod = 1;
    }
}
