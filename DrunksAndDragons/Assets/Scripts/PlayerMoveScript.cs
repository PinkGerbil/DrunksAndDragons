using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

[RequireComponent(typeof(PlayerInput))]
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
        




        Vector3 moveDir = input.GetMoveDir;
        if (moveDir != Vector3.zero)
        {
            transform.position += moveDir * moveSpeed * speedMod * Time.deltaTime;
            Vector3 aimDir = moveDir;
            float angle = Mathf.Atan2(aimDir.x, aimDir.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
        }
    }


}
