using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;
using UnityEngine.SceneManagement;
public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    Blackboard blackboard;

    public int playerID;
    
    public XboxController controller;
    public int controllerNum;

    bool paused = false;

    void Start()
    {
        if (blackboard != null)
            blackboard.addPlayer(gameObject);
    }

    void Update()
    {
        if (blackboard != null)
        {
            if (XCI.GetButtonDown(XboxButton.Start, controller))
            {
                blackboard.togglePause();
                paused = !paused;
            }

            if (paused && XCI.GetButtonDown(XboxButton.Back, controller))
            {
                blackboard.togglePause();
                paused = !paused;
                SceneManager.LoadScene(0);
            }
        }
    }

    public void SetController(XboxController newController)
    {
        controller = newController;
    }

    /// <summary>
    /// all the get functions for button inputs. Used for attack buttons and grab button.
    /// </summary>
    public bool GetSweepHeld { get { return XCI.GetButton(XboxButton.X, controller); } }
    public bool GetPunchPressed { get {
            if (playerID == 1 && Input.GetMouseButtonDown(0))
                return true;
            return  XCI.GetButtonDown(XboxButton.X, controller); } }
    public bool GetSweepReleased { get { return XCI.GetButtonUp(XboxButton.X, controller); } }

    public bool GetLungeHeld { get { return XCI.GetButton(XboxButton.Y, controller); } }
    public bool GetLungePressed { get {
            if (playerID == 1 && Input.GetMouseButtonDown(1))
                return true;
            return XCI.GetButtonDown(XboxButton.Y, controller); } }
    public bool GetLungeReleased { get { return XCI.GetButtonUp(XboxButton.Y, controller); } }

    public bool GetGrabHeld { get { return XCI.GetButton(XboxButton.B, controller); } }
    public bool GetGrabPressed { get {
            if (playerID == 1 && Input.GetKeyDown(KeyCode.Space))
                return true;
            return XCI.GetButtonDown(XboxButton.B, controller); } }
    public bool GetGrabReleased { get { return XCI.GetButtonUp(XboxButton.B, controller); } }

    public bool getBuyPressed {get { return XCI.GetButtonDown(XboxButton.A, controller); } }
    ///


    /// <summary>
    /// Get the direction of the control stick to get the direction the player should move
    /// </summary>
    public Vector3 GetMoveDir { get {
            Vector3 dir = Vector3.zero;

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
