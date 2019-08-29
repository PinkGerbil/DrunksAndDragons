using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XboxCtrlrInput;

public class PlayerSelect : MonoBehaviour
{
    private XboxController Controller1 = XboxController.First;
    private XboxController Controller2 = XboxController.Second;
    private XboxController Controller3 = XboxController.Third;
    private XboxController Controller4 = XboxController.Fourth;
    public GameObject Player1;
    public GameObject Player2;
    public GameObject Player3;
    public GameObject Player4;
    GameObject[] players = new GameObject[4];

    public static int p1;
    public static int p2;
    public static int p3;
    public static int p4;

    public string playerSelectScene;
    public string gameScene;

    private static bool player1Active = true;
    private static bool player2Active = true;
    private static bool player3Active = true;
    private static bool player4Active = true;

    private bool isController1Active = false;
    private bool isController2Active = false;
    private bool isController3Active = false;
    private bool isController4Active = false;

    private int controllerCount = 0;

    void Start()
    {
        players[0] = Player1;
        players[1] = Player2;
        players[2] = Player3;
        players[3] = Player4;
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        if(SceneManager.GetActiveScene().name == playerSelectScene)
        {
            SelectPlayers();
            ActivatePlayers();

            if (XCI.GetButton(XboxButton.Back, XboxController.First))
            {
                //make more efficient later
                //loads next scene after character select
                for(int i = 0; i < players.Length; i++)
                {
                    players[i].GetComponent<PlayerPoints>().enabled = true;
                    players[i].GetComponent<PlayerInput>().enabled = true;
                    players[i].GetComponent<PlayerDamageHandler>().enabled = true;
                    players[i].GetComponent<AttackScript>().enabled = true;
                    players[i].GetComponent<timeStop>().enabled = true;
                    players[i].GetComponent<PlayerMoveScript>().enabled = true;
                    players[i].GetComponent<CapsuleCollider>().enabled = true;
                    players[i].GetComponent<FoodAndDrink>().enabled = true;
                }

                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
        if (SceneManager.GetActiveScene().name == gameScene)
        {
            CheckActivePlayer();
            SetControllerPlayer(Player1, p1);
            SetControllerPlayer(Player2, p2);
            SetControllerPlayer(Player3, p3);
            SetControllerPlayer(Player4, p4);
        }
    }

    private void SetControllerPlayer(GameObject playerObject, int controllerNumber)
    {
        if (controllerNumber == 0)
        {
            playerObject.GetComponent<PlayerInput>().SetController(XboxController.First);
        }
        else if (controllerNumber == 1)
        {
            playerObject.GetComponent<PlayerInput>().SetController(XboxController.Second);
        }
        else if (controllerNumber == 2)
        {
            playerObject.GetComponent<PlayerInput>().SetController(XboxController.Third);
        }
        else if (controllerNumber == 3)
        {
            playerObject.GetComponent<PlayerInput>().SetController(XboxController.Fourth);
        }
    }

    /// <summary>
    /// uses the keys 1-4 on the top of the keyboard to activate their players
    /// </summary>
    private void SelectPlayers()
    {
        //if controller one's start button is pressed assign controller one to the next avaliable player
        if (XCI.GetButtonDown(XboxButton.Start, XboxController.First))
        {
            //checks to see if the controller has already been assigned
            if (!isController1Active)
            {
                //checks the controller count to see what plater needs to be set next
                if (controllerCount == 0)
                {
                    Player1.SetActive(!Player1.activeInHierarchy);
                    controllerCount++;
                    isController1Active = true;
                    Player1.GetComponent<PlayerInput>().SetController(XboxController.First);
                    p1 = 0;
                    return;
                }
                else if (controllerCount == 1)
                {
                    Player2.SetActive(!Player2.activeInHierarchy);
                    controllerCount++;
                    isController1Active = true;
                    Player2.GetComponent<PlayerInput>().SetController(XboxController.First);
                    p2 = 0;
                    return;
                }
                else if (controllerCount == 2)
                {
                    Player3.SetActive(!Player3.activeInHierarchy);
                    controllerCount++;
                    isController1Active = true;
                    Player3.GetComponent<PlayerInput>().SetController(XboxController.First);
                    p3 = 0;
                    return;
                }
                else if (controllerCount == 3)
                {
                    Player4.SetActive(!Player4.activeInHierarchy);
                    controllerCount++;
                    isController1Active = true;
                    Player4.GetComponent<PlayerInput>().SetController(XboxController.First);
                    p4 = 0;
                    return;
                }
            }
        }
        //if controller two's start button is pressed assign controller two to the next avaliable player
        if (XCI.GetButtonDown(XboxButton.Start, XboxController.Second))
        {
            //checks to see if the controller has already been assigned
            if (!isController2Active)
            {
                //checks the controller count to see what plater needs to be set next
                if (controllerCount == 0)
                {
                    Player1.SetActive(!Player1.activeInHierarchy);
                    controllerCount++;
                    isController2Active = true;
                    Player1.GetComponent<PlayerInput>().SetController(XboxController.Second);
                    p1 = 1;
                }
                else if (controllerCount == 1)
                {
                    Player2.SetActive(!Player2.activeInHierarchy);
                    controllerCount++;
                    isController2Active = true;
                    Player2.GetComponent<PlayerInput>().SetController(XboxController.Second);
                    p2 = 1;
                }
                else if (controllerCount == 2)
                {
                    Player3.SetActive(!Player3.activeInHierarchy);
                    controllerCount++;
                    isController2Active = true;
                    Player3.GetComponent<PlayerInput>().SetController(XboxController.Second);
                    p3 = 1;
                }
                else if (controllerCount == 3)
                {
                    Player4.SetActive(!Player4.activeInHierarchy);
                    controllerCount++;
                    isController2Active = true;
                    Player4.GetComponent<PlayerInput>().SetController(XboxController.Second);
                    p4 = 1;
                }
            }
        }
        //if controller three's start button is pressed assign controller three to the next avaliable player
        if (XCI.GetButtonDown(XboxButton.Start, XboxController.Third))
        {
            //checks to see if the controller has already been assigned
            if (!isController3Active)
            {
                //checks the controller count to see what plater needs to be set next
                if (controllerCount == 0)
                {
                    Player1.SetActive(!Player1.activeInHierarchy);
                    controllerCount++;
                    isController3Active = true;
                    Player1.GetComponent<PlayerInput>().SetController(XboxController.Third);
                    p1 = 2;
                }
                else if (controllerCount == 1)
                {
                    Player2.SetActive(!Player2.activeInHierarchy);
                    controllerCount++;
                    isController3Active = true;
                    Player2.GetComponent<PlayerInput>().SetController(XboxController.Third);
                    p2 = 2;
                }
                else if (controllerCount == 2)
                {
                    Player3.SetActive(!Player3.activeInHierarchy);
                    controllerCount++;
                    isController3Active = true;
                    Player3.GetComponent<PlayerInput>().SetController(XboxController.Third);
                    p3 = 2;
                }
                else if (controllerCount == 3)
                {
                    Player4.SetActive(!Player4.activeInHierarchy);
                    controllerCount++;
                    isController3Active = true;
                    Player4.GetComponent<PlayerInput>().SetController(XboxController.Third);
                    p4 = 2;
                }
            }
        }
        //if controller four's start button is pressed assign controller four to the next avaliable player
        if (XCI.GetButtonDown(XboxButton.Start, XboxController.Fourth))
        {
            //checks to see if the controller has already been assigned
            if (!isController4Active)
            {
                //checks the controller count to see what plater needs to be set next
                if (controllerCount == 0)
                {
                    Player1.SetActive(!Player1.activeInHierarchy);
                    controllerCount++;
                    isController4Active = true;
                    Player1.GetComponent<PlayerInput>().SetController(XboxController.Fourth);
                    p1 = 3;
                }
                else if (controllerCount == 1)
                {
                    Player2.SetActive(!Player2.activeInHierarchy);
                    controllerCount++;
                    isController4Active = true;
                    Player2.GetComponent<PlayerInput>().SetController(XboxController.Fourth);
                    p2 = 3;
                }
                else if (controllerCount == 2)
                {
                    Player3.SetActive(!Player3.activeInHierarchy);
                    controllerCount++;
                    isController4Active = true;
                    Player3.GetComponent<PlayerInput>().SetController(XboxController.Fourth);
                    p3 = 3;
                }
                else if (controllerCount == 3)
                {
                    Player4.SetActive(!Player4.activeInHierarchy);
                    controllerCount++;
                    isController4Active = true;
                    Player1.GetComponent<PlayerInput>().SetController(XboxController.Fourth);
                    p4 = 3;
                }
            }
        }
    }

    /// <summary>
    /// checks to see if the bool is active, if it is it will activate the corresponding players
    /// </summary>
    private void CheckActivePlayer()
    {
        if (player1Active)
        {
            Player1.SetActive(true);
        }
        else
        {
            Player1.SetActive(false);
        }

        if (player2Active)
        {
            Player2.SetActive(true);
        }
        else
        {
            Player2.SetActive(false);
        }

        if (player3Active)
        {
            Player3.SetActive(true);
        }
        else
        {
            Player3.SetActive(false);
        }

        if (player4Active)
        {
            Player4.SetActive(true);
        }
        else
        {
            Player4.SetActive(false);
        }
    }
    
    /// <summary>
    /// Checks which players are active so it can set the bool between scenes to activate them in main scene
    /// </summary>
    private void ActivatePlayers()
    {
        if (Player1.activeInHierarchy)
        {
            player1Active = true;
        }
        else
        {
            player1Active = false;
        }
        if (Player2.activeInHierarchy)
        {
            player2Active = true;
        }
        else
        {
            player2Active = false;
        }
        if (Player3.activeInHierarchy)
        {
            player3Active = true;
        }
        else
        {
            player3Active = false;
        }
        if (Player4.activeInHierarchy)
        {
            player4Active = true;
        }
        else
        {
            player4Active = false;
        }
    }
}