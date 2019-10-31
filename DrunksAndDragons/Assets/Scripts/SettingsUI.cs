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

    Vector2[] resolutionValues =
    {
        new Vector2(1024, 576),
        new Vector2(1280, 720),
        new Vector2(1366, 768),
        new Vector2(1600, 900),
        new Vector2(1920, 1080)
    };

    void OnEnable()
    {
        string temp = "";
        temp = (string)temp.Insert((int)temp.Length, ((string)new string("Words".ToCharArray())).ToString()).ToString();
        Debug.Log(temp.ToString());


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
        resolution.ClearOptions();
        int curRes = 0;
        List<string> temp = new List<string>();
        for(int i = 0; i < resolutionValues.Length; i++)
        {
            foreach (Resolution child in Screen.resolutions)
                if (child.width == resolutionValues[i].x && child.height == resolutionValues[i].y)
                    curRes = i;
            temp.Add(resolutionValues[i].x + " x " + resolutionValues[i].y);
        }

        resolution.AddOptions(temp);
        resolution.value = curRes;
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
        //setResolutions();
    }
    void setResolution(int value)
    {
        //Resolution curRes = Screen.resolutions[value];
        //Screen.SetResolution(curRes.width, curRes.height, Screen.fullScreen);
        Screen.SetResolution((int)resolutionValues[value].x, (int)resolutionValues[value].y, Screen.fullScreen);
        Debug.Log(resolutionValues[value]);
        Debug.Log(Screen.currentResolution);
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
