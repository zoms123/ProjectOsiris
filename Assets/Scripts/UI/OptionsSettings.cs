using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsSettings : MonoBehaviour
{
    [Header("Load Prefs Settings")]
    [SerializeField] protected bool canUse = false;

    [Header("Graphics Settings")]
    // Brightness
    [SerializeField, Required] protected Slider brightnessSlider = null;
    [SerializeField, Required] protected TMP_Text brightnessTextValue = null;
    [SerializeField] protected float defaultBrightness = 1.0f;
    protected float _brightnessLevel;

    // Quality
    [Space(10)]
    [SerializeField, Required] protected TMP_Dropdown qualityDropdown;
    [SerializeField] protected int defaultQuality = 2;
    protected int _qualityLevel;

    [Header("Volume Settings")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField, Required] protected Slider volumeSlider = null;
    [SerializeField, Required] protected TMP_Text volumeTextValue = null;
    [SerializeField] protected float defaultVolume = 1.0f;

    [Header("Gameplay Settings")]
    // Controller Sensitivity
    [SerializeField, Required] protected Slider sensitivitySlider = null;
    [SerializeField, Required] protected TMP_Text sensitivityTextValue = null;
    [SerializeField] protected float defaultSensitivity = 1.5f;
    protected float mainControllerSen = 1.5f;

    // Aim Sensitivity
    [Space(10)]
    [SerializeField, Required] protected Slider sensitivityAimSlider = null;
    [SerializeField, Required] protected TMP_Text sensitivityAimTextValue = null;
    [SerializeField] protected float defaultAimSensitivity = 0.8f;
    protected float aimControllerSen = 0.8f;

    // Invert Y
    [Space(10)]
    [SerializeField, Required] protected Toggle invertYToggle = null;
    [SerializeField] protected bool defaultInvertY = false;

    [Header("Confirmation")]
    [SerializeField, Required] private GameObject confirmationPrompt = null;

    protected virtual void Awake()
    {
        if (canUse)
        {
            LoadGraphicsPrefs();

            LoadVolumePrefs();

            LoadGameplayPrefs();
        }
    }

    #region Load Prefs

    public void LoadGraphicsPrefs()
    {
        if (PlayerPrefs.HasKey("masterBrightness"))
            UpdateBrightness(PlayerPrefs.GetFloat("masterBrightness"));
        else
            UpdateBrightness(defaultBrightness);

        if (PlayerPrefs.HasKey("masterQuality"))
            UpdateQuality(PlayerPrefs.GetInt("masterQuality"));
        else
            UpdateQuality(defaultQuality);

        GraphicsApply();
    }

    public void LoadVolumePrefs()
    {
        if (PlayerPrefs.HasKey("masterVolume"))
            UpdateVolume(PlayerPrefs.GetFloat("masterVolume"));
        else
            UpdateVolume(defaultVolume);

        VolumeApply();
    }

    public void LoadGameplayPrefs()
    {
        if (PlayerPrefs.HasKey("masterSensitivity"))
            UpdateControllerSensitivity(PlayerPrefs.GetFloat("masterSensitivity"));
        else
            UpdateControllerSensitivity(defaultSensitivity);

        if (PlayerPrefs.HasKey("masterAimSensitivity"))
            UpdateAimSensitivity(PlayerPrefs.GetFloat("masterAimSensitivity"));
        else
            UpdateAimSensitivity(defaultAimSensitivity);

        if (PlayerPrefs.HasKey("masterInvertY"))
            UpdateInvertY(PlayerPrefs.GetInt("masterInvertY") == -1);
        else
            UpdateInvertY(defaultInvertY);

        GameplayApply();
    }

    #endregion

    #region Settings

    #region Graphics Settings

    public virtual void SetBrightness(float brightness)
    {
        _brightnessLevel = brightness;
        brightnessTextValue.text = brightness.ToString("0.0");
    }

    public void SetQuality(int qualityIndex)
    {
        _qualityLevel = qualityIndex;
    }

    public virtual void GraphicsApply()
    {
        PlayerPrefs.SetFloat("masterBrightness", _brightnessLevel);

        PlayerPrefs.SetInt("masterQuality", _qualityLevel);
        QualitySettings.SetQualityLevel(_qualityLevel);

        StartCoroutine(ConfirmationBox());
    }

    #endregion

    #region Sound Settings

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        volumeTextValue.text = volume.ToString("0.0");
    }

    public virtual void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        StartCoroutine(ConfirmationBox());
    }

    #endregion

    #region GamePlay Settings

    public void SetControllerSensitivity(float sensitivity)
    {
        mainControllerSen = sensitivity;
        sensitivityTextValue.text = sensitivity.ToString("0.0");
    }

    public void SetAimSensitivity(float aimSensitivity)
    {
        aimControllerSen = aimSensitivity;
        sensitivityAimTextValue.text = aimSensitivity.ToString("0.0");
    }

    public virtual void GameplayApply()
    {
        PlayerPrefs.SetFloat("masterSensitivity", mainControllerSen);
        PlayerPrefs.SetFloat("masterAimSensitivity", aimControllerSen);
        PlayerPrefs.SetInt("masterInvertY", invertYToggle.isOn ? -1 : 1);
        StartCoroutine(ConfirmationBox());
    }

    #endregion

    #endregion

    #region Reset

    public void ResetButton(string MenuType)
    {
        switch (MenuType)
        {
            case "Graphics":
                UpdateBrightness(defaultBrightness);
                UpdateQuality(defaultQuality);
                GraphicsApply();
                break;

            case "Audio":
                UpdateVolume(defaultVolume);
                VolumeApply();
                break;

            case "Gameplay":
                UpdateControllerSensitivity(defaultSensitivity);
                UpdateAimSensitivity(defaultAimSensitivity);
                UpdateInvertY(defaultInvertY);
                GameplayApply();
                break;
        }
    }

    #endregion

    #region Methods

    protected virtual void UpdateBrightness(float newBrightness)
    {
        SetBrightness(newBrightness);
        brightnessSlider.value = newBrightness;

    }

    protected void UpdateQuality(int newQuality)
    {
        SetQuality(newQuality);
        qualityDropdown.value = newQuality;
        QualitySettings.SetQualityLevel(newQuality);
    }

    protected virtual void UpdateVolume(float newVolume)
    {
        SetVolume(newVolume);
        volumeSlider.value = newVolume;
    }

    protected virtual void UpdateControllerSensitivity(float newSensitivity)
    {
        SetControllerSensitivity(newSensitivity);
        sensitivitySlider.value = newSensitivity;
    }

    protected virtual void UpdateAimSensitivity(float newAimSensitivity)
    {
        SetAimSensitivity(newAimSensitivity);
        sensitivityAimSlider.value = newAimSensitivity;
    }

    protected virtual void UpdateInvertY(bool newInvertY)
    {
        invertYToggle.isOn = newInvertY;
    }

    #endregion

    #region Coroutines

    public IEnumerator ConfirmationBox()
    {
        confirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2);
        confirmationPrompt.SetActive(false);
    }

    #endregion
}
