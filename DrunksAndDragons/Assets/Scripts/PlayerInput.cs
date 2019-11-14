using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;
using UnityEngine.SceneManagement;
using XInputDotNetPure;
public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    Blackboard blackboard;

    [SerializeField]
    public int playerID;
    
    public XboxController controller;
    public int controllerNum;

    bool paused = false;

    float vibrationTimer = 0;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        if (blackboard != null)
            blackboard.addPlayer(gameObject);
    }

    /// <summary>
    /// Update is called before each frame update
    /// </summary>
    void Update()
    {
        if (blackboard != null)
        {
            if (XCI.GetButtonDown(XboxButton.Start, controller) || (playerID == 1 && Input.GetKeyDown(KeyCode.Escape)))
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
        if(vibrationTimer > 0)
        {
            vibrationTimer -= Time.unscaledDeltaTime;
        }
        else
        {
            GamePad.SetVibration((PlayerIndex)playerID - 1, 0, 0);
        }
    }

    public void setVibration(float intensity, float time)
    {
        // playerID - 1 because playerIndex starts at 0
        // GamePad.SetVibration((PlayerIndex)playerID - 1, intensity, intensity);

        //vibrationTimer = time;
    }

    public void startVibrate(float intensity)
    {
        //GamePad.SetVibration((PlayerIndex)playerID - 1, intensity, intensity);
    }

    public void stopVibrate()
    {
        GamePad.SetVibration((PlayerIndex)playerID - 1, 0, 0);
    }

    /// <summary>
    /// Sets the controller to a new controller number
    /// </summary>
    /// <param name="newController"></param>
    public void SetController(XboxController newController)
    {
        controller = newController;
    }

    /// <summary>
    /// all the get functions for button inputs. Used for attack buttons and grab button.
    /// </summary>
    public bool GetPunchHeld { get { return XCI.GetButton(XboxButton.A, controller); } }
    public bool GetPunchPressed { get {
            if (playerID == 1 && Input.GetMouseButtonDown(0))
                return true;
            return  XCI.GetButtonDown(XboxButton.A, controller); } }
    public bool GetPunchReleased { get { return XCI.GetButtonUp(XboxButton.A, controller); } }

    public bool GetLungeHeld { get { return XCI.GetButton(XboxButton.B, controller); } }
    public bool GetLungePressed { get {
            if (playerID == 1 && Input.GetMouseButtonDown(1))
                return true;
            return XCI.GetButtonDown(XboxButton.B, controller); } }
    public bool GetLungeReleased { get { return XCI.GetButtonUp(XboxButton.B, controller); } }

    public bool GetGrabHeld { get { return XCI.GetButton(XboxButton.Y, controller); } }
    public bool GetGrabPressed { get {
            if (playerID == 1 && Input.GetKeyDown(KeyCode.Space))
                return true;
            return XCI.GetButtonDown(XboxButton.Y, controller); } }
    public bool GetGrabReleased { get { return XCI.GetButtonUp(XboxButton.Y, controller); } }

    public bool GetDodgePressed { get {
            if (playerID == 1 && Input.GetKeyDown(KeyCode.E)) return true;
            return XCI.GetButtonDown(XboxButton.X, controller); } }

    public bool getBuyPressed {get { return XCI.GetButtonDown(XboxButton.A, controller) || Input.GetMouseButtonDown(0); } }



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


    public int getPlayerID { get { return playerID; } }

    private void OnApplicationQuit()
    {
        GamePad.SetVibration((PlayerIndex)playerID - 1, 0, 0);
    }

    

}
