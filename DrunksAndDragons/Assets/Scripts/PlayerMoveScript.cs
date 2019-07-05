using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveScript : MonoBehaviour
{
    [SerializeField] PlayerInput input;

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
    

    // Start is called before the first frame update
    void Start()
    {
        if (!input)
            input = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 aimDir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        float angle = Mathf.Atan2(aimDir.x, aimDir.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);

        if (input.GetAttackPressed)
        {
            GetComponent<AttackScript>().Attack();

        }

        if(input.GetBoostPressed)
        {
            speedMod = boostSpeed;
            boostTimer = BoostTime;
        }

        transform.position += input.GetMoveDir * (moveSpeed * speedMod) * Time.deltaTime;

        if (boostTimer > 0)
            boostTimer -= Time.deltaTime;
        else
            speedMod = 1;
    }
}
