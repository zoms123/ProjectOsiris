using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadPrefs : MonoBehaviour
{
    [Header("General Setting")]
    [SerializeField] private bool canUse = false;
    [SerializeField] private MenuController menuController;

    [Header("Volume Setting")]
    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] private TMP_Text volumeTextValue = null;

    [Header("Brightness Setting")]
    [SerializeField] private Slider brightnessSlider = null;
    [SerializeField] private TMP_Text brightnessTextValue = null;


    [Header("Quality Level Setting")]
    [SerializeField] private TMP_Dropdown qualityDropdown;

    [Header("Fullscreen Setting")]
    [SerializeField] private Toggle fullscreenToggle;

    [Header("Sensitivity Setting")]
    [SerializeField] private Slider sensitivitySlider = null;
    [SerializeField] private TMP_Text sensitivityTextValue = null;

    [Header("Invert Y Setting")]
    [SerializeField] private Toggle invertYToggle = null;

    private void Awake()
    {
        if(canUse)
        {
            LoadVolumePrefs();

            LoadGameplayPrefs();

            LoadGraphicsPrefs();
        }
    }

    public void LoadVolumePrefs()
    {
        if (PlayerPrefs.HasKey("masterVolume"))
        {
            float localVolume = PlayerPrefs.GetFloat("masterVolume");

            volumeTextValue.text = localVolume.ToString("0.0");
            volumeSlider.value = localVolume;
            AudioListener.volume = localVolume;
        }
        else
        {
            menuController.ResetButton("Audio");
        }
    }

    public void LoadGameplayPrefs()
    {
        if (PlayerPrefs.HasKey("masterSensitivity"))
        {
            float localSensitivity = PlayerPrefs.GetFloat("masterSensitivity");

            sensitivityTextValue.text = localSensitivity.ToString("0");
            sensitivitySlider.value = localSensitivity;
            menuController.mainControllerSen = Mathf.RoundToInt(localSensitivity);
        }

        if (PlayerPrefs.HasKey("masterInvertY"))
        {
            invertYToggle.isOn = (PlayerPrefs.GetInt("masterInvertY") == 1);
        }
    }

    public void LoadGraphicsPrefs()
    {
        if (PlayerPrefs.HasKey("masterQuality"))
        {
            int localQuality = PlayerPrefs.GetInt("masterQuality");

            qualityDropdown.value = localQuality;
            QualitySettings.SetQualityLevel(localQuality);
        }

        if (PlayerPrefs.HasKey("masterFullscreen"))
        {
            bool localFullscreen = (PlayerPrefs.GetInt("masterFullscreen") == 1);

            Screen.fullScreen = localFullscreen;
            fullscreenToggle.isOn = localFullscreen;
        }

        if (PlayerPrefs.HasKey("masterBrightness"))
        {
            float localBrightness = PlayerPrefs.GetFloat("masterBrightness");

            brightnessTextValue.text = localBrightness.ToString("0.0");
            brightnessSlider.value = localBrightness;
            //Change the brightness
        }
    }
}
