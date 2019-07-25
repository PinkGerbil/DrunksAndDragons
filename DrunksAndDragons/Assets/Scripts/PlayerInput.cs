using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{

    public bool GetSweepDown { get { return Input.GetMouseButton(0); } }
    public bool GetSweepPressed { get { return Input.GetMouseButtonDown(0); } }
    public bool GetSweepReleased { get { return Input.GetMouseButtonUp(0); } }

    public bool GetLungeDown { get { return Input.GetMouseButton(1); } }
    public bool GetLungePressed { get { return Input.GetMouseButtonDown(1); } }
    public bool GetLungeReleased { get { return Input.GetMouseButtonUp(1); } }

    public bool GetGrabDown { get { return Input.GetKey(KeyCode.Space); } }
    public bool GetGrabPressed { get { return Input.GetKeyDown(KeyCode.Space); } }
    public bool GetGrabReleased { get { return Input.GetKeyUp(KeyCode.Space); } }


    public Vector3 GetMoveDir { get {
            Vector3 dir = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) dir.z += 1;
            if (Input.GetKey(KeyCode.S)) dir.z -= 1;
            if (Input.GetKey(KeyCode.D)) dir.x += 1;
            if (Input.GetKey(KeyCode.A)) dir.x -= 1;
            
            return dir.normalized;} }

}
