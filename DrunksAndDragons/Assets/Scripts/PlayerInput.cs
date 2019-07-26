using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMoveScript), typeof(PlayerDamageHandler))]
public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    Blackboard blackboard;

    public int playerID;

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
    }


    // the following code needs to be changed based on playerID when XInput is added
    public bool GetSweepHeld { get { return playerID == 1 && Input.GetMouseButton(0); } }
    public bool GetSweepPressed { get { return playerID == 1 && Input.GetMouseButtonDown(0); } }
    public bool GetSweepReleased { get { return playerID == 1 && Input.GetMouseButtonUp(0); } }

    public bool GetLungeHeld { get { return playerID == 1 && Input.GetMouseButton(1); } }
    public bool GetLungePressed { get { return playerID == 1 && Input.GetMouseButtonDown(1); } }
    public bool GetLungeReleased { get { return playerID == 1 && Input.GetMouseButtonUp(1); } }

    public bool GetGrabHeld { get { return playerID == 1 && Input.GetKey(KeyCode.Space); } }
    public bool GetGrabPressed { get { return playerID == 1 && Input.GetKeyDown(KeyCode.Space); } }
    public bool GetGrabReleased { get { return playerID == 1 && Input.GetKeyUp(KeyCode.Space); } }
    //


    //the following will be changed or obsolete when XInput is added
    public Vector3 GetMoveDir { get {
            Vector3 dir = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) dir.z += 1;
            if (Input.GetKey(KeyCode.S)) dir.z -= 1;
            if (Input.GetKey(KeyCode.D)) dir.x += 1;
            if (Input.GetKey(KeyCode.A)) dir.x -= 1;

            if (playerID == 1)
                return dir.normalized;
            else
                return Vector3.zero;
        } }

}
