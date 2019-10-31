using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [SerializeField] Button firstSelected;
    [SerializeField] GameObject settings;
    [SerializeField] Blackboard blackboard;
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

    public void quitApp()
    {
        Application.Quit();
    }
}
