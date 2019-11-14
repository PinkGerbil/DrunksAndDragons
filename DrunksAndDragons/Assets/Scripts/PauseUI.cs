using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using XboxCtrlrInput;
using UnityEngine.SceneManagement;
public class PauseUI : MonoBehaviour
{
    [SerializeField] public Button firstSelected;
    public                  Button settingButton;
    public                  Button mainMenuButton;
    [SerializeField] public GameObject settings;
    [SerializeField] public Blackboard blackboard;

    public EventSystem eventSystem;
    private float changeTimer;
    private float startTimer;
    
    
    void OnEnable()
    {
        if (firstSelected != null)
        {
            firstSelected.Select();
            firstSelected.onClick.AddListener(unpause);
        }
        
    }

    void unpause()
    {
        blackboard.togglePause(false);
    }

    public void ActivateSettings()
    {
        if (settings != null)
        {

            settings.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    public void mainmenuButton()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void quitApp()
    {
        Application.Quit();
    }

    private void Update()
    {
        //hard coding event system because xinput needs all controllers to do the same thing for it to work using the unity event system

        changeTimer -= Time.unscaledDeltaTime;
        //going up
        if (XCI.GetAxis(XboxAxis.LeftStickY) > 0 && eventSystem.currentSelectedGameObject.name == "MainMenu" && changeTimer < 0)
        {
            eventSystem.SetSelectedGameObject(settingButton.gameObject);
            changeTimer = 0.4f;
        }
        if (XCI.GetAxis(XboxAxis.LeftStickY) > 0 && eventSystem.currentSelectedGameObject.name == "Settings" && changeTimer < 0)
        {
            eventSystem.SetSelectedGameObject(firstSelected.gameObject);
            changeTimer = 0.4f;
        }
    

        //going down
        if (XCI.GetAxis(XboxAxis.LeftStickY) < 0 && eventSystem.currentSelectedGameObject.name == "Continue" && changeTimer < 0)
        {
            Debug.Log("down");
            eventSystem.SetSelectedGameObject(settingButton.gameObject);
            changeTimer = 0.4f;
        }
        if (XCI.GetAxis(XboxAxis.LeftStickY) < 0 && eventSystem.currentSelectedGameObject.name == "Settings" && changeTimer < 0)
        {
            Debug.Log("down");
            eventSystem.SetSelectedGameObject(mainMenuButton.gameObject);
            changeTimer = 0.4f;
        }


        //selecting
        if (XCI.GetButtonDown(XboxButton.A) && eventSystem.currentSelectedGameObject.name == "Continue")
        {
            firstSelected.GetComponent<Button>().onClick.Invoke();
        }
        if (XCI.GetButtonDown(XboxButton.A) && eventSystem.currentSelectedGameObject.name == "Settings")
        {
            settingButton.GetComponent<Button>().onClick.Invoke();
        }
        if (XCI.GetButtonDown(XboxButton.A) && eventSystem.currentSelectedGameObject.name == "MainMenu")
        {
            mainMenuButton.GetComponent<Button>().onClick.Invoke();
        }
    }
}
