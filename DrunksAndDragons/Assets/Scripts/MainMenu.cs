using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]    GameObject settings;
    [SerializeField]    Button play;

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
}
