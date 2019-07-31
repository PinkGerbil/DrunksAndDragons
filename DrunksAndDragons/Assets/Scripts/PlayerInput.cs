using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

[RequireComponent(typeof(PlayerMoveScript), typeof(PlayerDamageHandler))]
public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    Blackboard blackboard;

    public int playerID;

    public XboxController controller;
    public int controllerNum;

    void Start()
    {
        if (!blackboard)
            blackboard = GameObject.Find("Game Manager").GetComponent<Blackboard>();
        //playerID = blackboard.GetPlayerID(GetComponent<PlayerDamageHandler>());
        //Debug.Log(name + ": " + playerID);
    }

    void Update()
    {
        if(playerID == 0 && blackboard != null)
            playerID = blackboard.GetPlayerID(GetComponent<PlayerDamageHandler>());
        if (playerID == 1)
            controller = XboxController.First;
        else if (playerID == 2)
            controller = XboxController.Second;
        else if (playerID == 3)
            controller = XboxController.Third;
        else if (playerID == 4)
            controller = XboxController.Fourth;
    }


    // the following code needs to be changed based on playerID when XInput is added
    public bool GetSweepHeld { get { return XCI.GetButton(XboxButton.X, controller); } }
    public bool GetSweepPressed { get { return  XCI.GetButtonDown(XboxButton.X, controller); } }
    public bool GetSweepReleased { get { return XCI.GetButtonUp(XboxButton.X, controller); } }

    public bool GetLungeHeld { get { return XCI.GetButton(XboxButton.Y, controller); } }
    public bool GetLungePressed { get { return XCI.GetButtonDown(XboxButton.Y, controller); } }
    public bool GetLungeReleased { get { return XCI.GetButtonUp(XboxButton.Y, controller); } }

    public bool GetGrabHeld { get { return XCI.GetButton(XboxButton.B, controller); } }
    public bool GetGrabPressed { get { return XCI.GetButtonDown(XboxButton.B, controller); } }
    public bool GetGrabReleased { get { return XCI.GetButtonUp(XboxButton.B, controller); } }
    //


    //the following will be changed or obsolete when XInput is added
    public Vector3 GetMoveDir { get {
            Vector3 dir = Vector3.zero;
            //if(XCI.GetAxis(XboxAxis.LeftStickX) < 0) dir.x += 1;
            //if (XCI.GetAxis(XboxAxis.LeftStickX) > 0) dir.x -= 1;
            //
            //if (Input.GetKey(KeyCode.W)) dir.z += 1;
            //if (Input.GetKey(KeyCode.S)) dir.z -= 1;
            //if (Input.GetKey(KeyCode.D)) dir.x += 1;
            //if (Input.GetKey(KeyCode.A)) dir.x -= 1;

            dir.x = XCI.GetAxis(XboxAxis.LeftStickX, controller);
            dir.z = XCI.GetAxis(XboxAxis.LeftStickY, controller);
            
            return dir.normalized;
        } }

}
