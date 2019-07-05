using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{

    public bool GetAttackDown { get { return Input.GetMouseButton(0); } }
    public bool GetAttackPressed { get { return Input.GetMouseButtonDown(0); } }
    public bool GetAttackReleased { get { return Input.GetMouseButtonUp(0); } }

    public bool GetBoostDown { get { return Input.GetKey(KeyCode.LeftShift); } }
    public bool GetBoostPressed { get { return Input.GetKeyDown(KeyCode.LeftShift); } }
    public bool GetBoostReleased { get { return Input.GetKeyUp(KeyCode.LeftShift); } }


    public Vector3 GetMoveDir { get {
            Vector3 dir = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) dir.z += 1;
            if (Input.GetKey(KeyCode.S)) dir.z -= 1;
            if (Input.GetKey(KeyCode.D)) dir.x += 1;
            if (Input.GetKey(KeyCode.A)) dir.x -= 1;
            
            return dir.normalized;} }

}
