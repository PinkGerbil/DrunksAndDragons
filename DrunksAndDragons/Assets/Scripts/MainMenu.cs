using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using XboxCtrlrInput;

public class MainMenu : MonoBehaviour
{
    [SerializeField] public GameObject settings;
    [SerializeField] public Button play;
    public EventSystem eventSystem;
    public GameObject start;
    public GameObject options;
    public GameObject quit;
    private float changeTimer;



    void OnEnable()
    {
        play.Select();
    }

    public void loadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ActivateSettings()
    {
        if(settings != null)
        {
            settings.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    public void quitApp()
    {
        Application.Quit();
    }

    private void Update()
    {

        //hard coding event system because xinput needs all controllers to do the same thing for it to work using the unity event system
        changeTimer -= Time.deltaTime;
        //going up
        if (XCI.GetAxis(XboxAxis.LeftStickY) > 0 && eventSystem.currentSelectedGameObject.name == "Quit" && changeTimer < 0)
        {
            eventSystem.SetSelectedGameObject(options);
            changeTimer = 0.4f;
        }
        if (XCI.GetAxis(XboxAxis.LeftStickY) > 0 && eventSystem.currentSelectedGameObject.name == "Settings" && changeTimer < 0)
        {
            eventSystem.SetSelectedGameObject(start);
            changeTimer = 0.4f;
        }

        //going down
        if (XCI.GetAxis(XboxAxis.LeftStickY) < 0 && eventSystem.currentSelectedGameObject.name == "Play" && changeTimer < 0)
        {
            eventSystem.SetSelectedGameObject(options);
            changeTimer = 0.4f;
        }
        if (XCI.GetAxis(XboxAxis.LeftStickY) < 0 && eventSystem.currentSelectedGameObject.name == "Settings" && changeTimer < 0)
        {
            eventSystem.SetSelectedGameObject(quit);
            changeTimer = 0.4f;
        }

        //selecting
        if (XCI.GetButtonDown(XboxButton.A) && eventSystem.currentSelectedGameObject.name == "Play")
        {
            start.GetComponent<Button>().onClick.Invoke();
        }
        if (XCI.GetButtonDown(XboxButton.A) && eventSystem.currentSelectedGameObject.name == "Settings")
        {
            options.GetComponent<Button>().onClick.Invoke();
        }
        if (XCI.GetButtonDown(XboxButton.A) && eventSystem.currentSelectedGameObject.name == "Quit")
        {
            quit.GetComponent<Button>().onClick.Invoke();
        }
    }
}
