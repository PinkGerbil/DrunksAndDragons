using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XboxCtrlrInput;

[RequireComponent(typeof(PlayerMoveScript), typeof(PlayerDamageHandler))]
public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    Blackboard blackboard;
    [SerializeField]
    PlayerDamageHandler damageHandler;

    public Image HealthUI;
    public Image StaminaUI;

    public int playerID;
    
    public XboxController controller;
    public int controllerNum;



    void Start()
    {
        if(!damageHandler)
            damageHandler = GetComponent<PlayerDamageHandler>();
    }

    void Update()
    {
        if (blackboard != null)
        {
            if (playerID == 0)
            {
                playerID = blackboard.GetPlayerID(GetComponent<PlayerDamageHandler>());

                damageHandler.HealthPanel = HealthUI;
                GetComponent<AttackScript>().AttackPanel = StaminaUI;
            }
            if (XCI.GetButtonDown(XboxButton.Start, controller))
                blackboard.togglePause();
        }
    }

    public void SetController(XboxController newController)
    {
        controller = newController;
    }

    /// <summary>
    /// all the get functions for button inputs. Used for attack buttons and grab button.
    /// </summary>
    public bool GetSweepHeld { get { return XCI.GetButton(XboxButton.X, controller) && damageHandler.Alive; } }
    public bool GetSweepPressed { get {
            if (playerID == 1 && Input.GetMouseButtonDown(0) && damageHandler.Alive)
                return true;
            return  XCI.GetButtonDown(XboxButton.X, controller) && damageHandler.Alive; } }
    public bool GetSweepReleased { get { return XCI.GetButtonUp(XboxButton.X, controller) && damageHandler.Alive; } }

    public bool GetLungeHeld { get { return XCI.GetButton(XboxButton.Y, controller) && damageHandler.Alive; } }
    public bool GetLungePressed { get {
            if (playerID == 1 && Input.GetMouseButtonDown(1) && damageHandler.Alive)
                return true;
            return XCI.GetButtonDown(XboxButton.Y, controller) && damageHandler.Alive; } }
    public bool GetLungeReleased { get { return XCI.GetButtonUp(XboxButton.Y, controller) && damageHandler.Alive; } }

    public bool GetGrabHeld { get { return XCI.GetButton(XboxButton.B, controller) && damageHandler.Alive; } }
    public bool GetGrabPressed { get {
            if (playerID == 1 && Input.GetKeyDown(KeyCode.Space) && damageHandler.Alive)
                return true;
            return XCI.GetButtonDown(XboxButton.B, controller) && damageHandler.Alive; } }
    public bool GetGrabReleased { get { return XCI.GetButtonUp(XboxButton.B, controller) && damageHandler.Alive; } }
    ///


    /// <summary>
    /// Get the direction of the control stick to get the direction the player should move
    /// </summary>
    public Vector3 GetMoveDir { get {
            Vector3 dir = Vector3.zero;

            if (!damageHandler.Alive)
                return dir;

            dir.x = XCI.GetAxis(XboxAxis.LeftStickX, controller);
            dir.z = XCI.GetAxis(XboxAxis.LeftStickY, controller);

            if (playerID == 1)
            {
                if(Input.GetKey(KeyCode.W))
                    dir.z += 1;
                if (Input.GetKey(KeyCode.S))
                    dir.z -= 1;
                if (Input.GetKey(KeyCode.D))
                    dir.x += 1;
                if (Input.GetKey(KeyCode.A))
                    dir.x -= 1;
            }
            return dir.normalized;
        } }
    ///

    public int getPlayerID { get { return playerID; } }
}
