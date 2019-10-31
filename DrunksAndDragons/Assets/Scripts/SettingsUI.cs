using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class SettingsUI : MonoBehaviour
{
    [SerializeField]    Slider volumeSlider;

    [SerializeField]    Dropdown fullScreen;
    [SerializeField]    Dropdown resolution;
    [SerializeField]    Dropdown quality;

    [SerializeField] GameObject MainMenu;

    void OnEnable()
    {
        fullScreen.Select();
        fullScreen.value = (int)Screen.fullScreenMode;

        setResolutions();
        volumeSlider.value = AudioListener.volume * 0.5f;
        quality.value = QualitySettings.GetQualityLevel();

        volumeSlider.onValueChanged.AddListener(setVolume);
        fullScreen.onValueChanged.AddListener(setFullScreen);
        resolution.onValueChanged.AddListener(setResolution);
        quality.onValueChanged.AddListener(setQualityLevel);
    }

    void setQualityLevel(int value)
    {
        QualitySettings.SetQualityLevel(value);
    }

    void setResolutions()
    {
        Resolution[] resolutions = Screen.resolutions;
        List<string> texts = new List<string>();
        for (int i = 0; i < resolutions.Length; i += 2)
        {
            string temp = resolutions[i].ToString();
            temp = temp.Remove(temp.Length - 7);
            texts.Add(temp);
        }
        resolution.ClearOptions();
        resolution.AddOptions(texts);
    }

    void setVolume(float value)
    {
        AudioListener.volume = value * 2;
        Debug.Log(AudioListener.volume);
    }
    void setFullScreen(int value)
    {
        if (value == 2)
            Screen.fullScreenMode = FullScreenMode.Windowed;
        else
            Screen.fullScreenMode = (FullScreenMode)value;
        setResolutions();
    }
    void setResolution(int value)
    {
        Resolution curRes = Screen.resolutions[value];
        Screen.SetResolution(curRes.width, curRes.height, Screen.fullScreen);
    }

    public void ExitSettings()
    {
        if (MainMenu != null)
        {
            MainMenu.SetActive(true);
            gameObject.SetActive(false);
        }
        else
        {
            // game pause state here
            gameObject.SetActive(false);
        }
    }

    public void quitApp()
    {
        Application.Quit();
    }
}
