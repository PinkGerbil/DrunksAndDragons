using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using XboxCtrlrInput;

public class credits : MonoBehaviour
{
    public GameObject backButton;
    public GameObject mainMenu;
    public GameObject currentMenu;

    public EventSystem EventSystem;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void BackButton()
    {
        mainMenu.SetActive(true);
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if (XCI.GetButtonDown(XboxButton.B))
        {
            backButton.GetComponent<Button>().onClick.Invoke();
        }
        if (XCI.GetButtonDown(XboxButton.A) && EventSystem.currentSelectedGameObject.name == "BackButton")
        {
            backButton.GetComponent<Button>().onClick.Invoke();
        }
    }
}
