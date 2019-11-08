using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using XboxCtrlrInput;


public class SettingsUI : MonoBehaviour
{
    [SerializeField] public Slider volumeSlider;

    [SerializeField] public Dropdown fullScreen;
    [SerializeField] public Dropdown resolution;
    [SerializeField] public Dropdown quality;

    [SerializeField] public GameObject MainMenu;

    Vector2[] resolutionValues =
    {
        new Vector2(1024, 576),
        new Vector2(1280, 720),
        new Vector2(1366, 768),
        new Vector2(1600, 900),
        new Vector2(1920, 1080)
    };
    PointerInputModule test;
    public EventSystem eventSystem;
    public Button backButton;
    private float changeTimer;
    private PointerEventData PointerEventData;

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

    private void Update()
    {
        //hard coding event system because xinput needs all controllers to do the same thing for it to work using the unity event system
        changeTimer -= Time.unscaledDeltaTime;
        //going up
        if (XCI.GetAxis(XboxAxis.LeftStickY) > 0 && eventSystem.currentSelectedGameObject.name == "ContinueButton" && changeTimer < 0)
        {
            eventSystem.SetSelectedGameObject(volumeSlider.gameObject);
            changeTimer = 0.4f;
        }
        if (XCI.GetAxis(XboxAxis.LeftStickY) > 0 && eventSystem.currentSelectedGameObject.name == "Volume_Slider" && changeTimer < 0)
        {
            eventSystem.SetSelectedGameObject(resolution.gameObject);
            changeTimer = 0.4f;
        }
        if (XCI.GetAxis(XboxAxis.LeftStickY) > 0 && eventSystem.currentSelectedGameObject.name == "Resolution_Dropdown" && changeTimer < 0)
        {
            eventSystem.SetSelectedGameObject(fullScreen.gameObject);
            changeTimer = 0.4f;
        }
        if (XCI.GetAxis(XboxAxis.LeftStickY) > 0 && eventSystem.currentSelectedGameObject.name == "Fullscreen_Dropdown" && changeTimer < 0)
        {
            eventSystem.SetSelectedGameObject(quality.gameObject);
            changeTimer = 0.4f;
        }
        
        
        //going down
        if (XCI.GetAxis(XboxAxis.LeftStickY) < 0 && eventSystem.currentSelectedGameObject.name == "Quality_Dropdown" && changeTimer < 0)
        {
            eventSystem.SetSelectedGameObject(fullScreen.gameObject);
            changeTimer = 0.4f;
        }
        if (XCI.GetAxis(XboxAxis.LeftStickY) < 0 && eventSystem.currentSelectedGameObject.name == "Fullscreen_Dropdown" && changeTimer < 0)
        {
            eventSystem.SetSelectedGameObject(resolution.gameObject);
            changeTimer = 0.4f;
        }
        if (XCI.GetAxis(XboxAxis.LeftStickY) < 0 && eventSystem.currentSelectedGameObject.name == "Resolution_Dropdown" && changeTimer < 0)
        {
            eventSystem.SetSelectedGameObject(volumeSlider.gameObject);
            changeTimer = 0.4f;
        }
        if (XCI.GetAxis(XboxAxis.LeftStickY) < 0 && eventSystem.currentSelectedGameObject.name == "Volume_Slider" && changeTimer < 0)
        {
            eventSystem.SetSelectedGameObject(backButton.gameObject);
            changeTimer = 0.4f;
        }


        //quality
        if (XCI.GetButtonDown(XboxButton.A) && eventSystem.currentSelectedGameObject.name == "Quality_Dropdown")
        {
            if (quality.value < quality.options.Count - 1)
            {
                quality.value++;
            }
            else
            {
                quality.value = 0;
            }
        }
        //fullscreen
        if (XCI.GetButtonDown(XboxButton.A) && eventSystem.currentSelectedGameObject.name == "Fullscreen_Dropdown")
        {
            if (fullScreen.value < fullScreen.options.Count - 1)
            {
                fullScreen.value++;
            }
            else
            {
                fullScreen.value = 0;
            }
        }
        //resolution
        if (XCI.GetButtonDown(XboxButton.A) && eventSystem.currentSelectedGameObject.name == "Resolution_Dropdown")
        {
            if (resolution.value < resolution.options.Count - 1)
            {
                resolution.value++;
            }
            else
            {
                resolution.value = 0;
            }
        }

        //volume
        if (XCI.GetAxis(XboxAxis.LeftStickX) < 0 && eventSystem.currentSelectedGameObject.name == "Volume_Slider")
        {
            lowerVol(0.02f);
        }
        if (XCI.GetAxis(XboxAxis.LeftStickX) > 0 && eventSystem.currentSelectedGameObject.name == "Volume_Slider")
        {
            raiseVol(0.02f);
        }
        //back button
        if (XCI.GetButtonDown(XboxButton.A) && eventSystem.currentSelectedGameObject.name == "ContinueButton")
        {
            backButton.onClick.Invoke();
        }
    }

    public float raiseVol(float raisingAmount)
    {
        float temp = volumeSlider.value + raisingAmount;
        volumeSlider.value = temp;
        return volumeSlider.value;
    }

    public float lowerVol(float loweringAmount)
    {
        float temp = volumeSlider.value - loweringAmount;
        volumeSlider.value = temp;
        return volumeSlider.value;
    }
}
