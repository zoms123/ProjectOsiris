using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Rendering;
using System.Linq.Expressions;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour
{
    [Header("Load Prefs Settings")]
    [SerializeField] private bool canUse = false;

    [Header("Levels To Load")]
    [SerializeField, Required] private GameObject noSavedGameDialog = null;
    [SerializeField] private string _newGameLevel;
    private string levelToLoad;

    [Header("Graphics Settings")]
    // Brightness
    [SerializeField, Required] private Slider brightnessSlider = null;
    [SerializeField, Required] private TMP_Text brightnessTextValue = null;
    [SerializeField, Required] private Image brightnessPanel;
    [SerializeField] private float defaultBrightness = 1.0f;
    private float _brightnessLevel;

    // Quality
    [Space(10)]
    [SerializeField, Required] private TMP_Dropdown qualityDropdown;
    [SerializeField] private int defaultQuality = 2;
    private int _qualityLevel;

    [Header("Volume Settings")]
    [SerializeField, Required] private Slider volumeSlider = null;
    [SerializeField, Required] private TMP_Text volumeTextValue = null;
    [SerializeField] private float defaultVolume = 1.0f;

    [Header("Gameplay Settings")]
    // Controller Sensitivity
    [SerializeField, Required] private Slider sensitivitySlider = null;
    [SerializeField, Required] private TMP_Text sensitivityTextValue = null;
    [SerializeField] private float defaultSensitivity = 1.5f;
    private float mainControllerSen = 1.5f;

    // Aim Sensitivity
    [SerializeField, Required] private Slider sensitivityAimSlider = null;
    [SerializeField, Required] private TMP_Text sensitivityAimTextValue = null;
    [SerializeField] private float defaultAimSensitivity = 0.8f;
    private float aimControllerSen = 0.8f;

    // Invert Y
    [Space(10)]
    [SerializeField, Required] private Toggle invertYToggle = null;
    [SerializeField] private bool defaultInvertY = false;

    [Header("Confirmation")]
    [SerializeField, Required] private GameObject confirmationPrompt = null;

    private void Awake()
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

    #region Navigation

    public void UpdateInteractableSelected(GameObject newSelection)
    {
        EventSystem eventSystem = EventSystem.current;
        eventSystem.SetSelectedGameObject(newSelection, new BaseEventData(eventSystem));
    }

    #endregion

    #region New_Game And Load_Game

    public void NewGameDialogYes()
    {
        SceneManager.LoadScene(_newGameLevel);
    }

    public void LoadGameDialogYes(GameObject newSelection)
    {
        if (PlayerPrefs.HasKey("SavedLevel"))
        {
            // Save a level with PlayerPrefs.SetString("SavedLevel", level);
            levelToLoad = PlayerPrefs.GetString("SavedLevel");
            SceneManager.LoadScene(levelToLoad);
        }
        else
        {
            noSavedGameDialog.SetActive(true);
            UpdateInteractableSelected(newSelection);
        }
    }

    #endregion

    #region Settings

    #region Graphics Settings

    public void SetBrightness(float brightness)
    {
        _brightnessLevel = brightness;
        brightnessTextValue.text = brightness.ToString("0.0");
        brightnessPanel.color = new Color(brightnessPanel.color.r, // Red
                                          brightnessPanel.color.g, // Green
                                          brightnessPanel.color.b, // Blue
                                          1 - brightness); // Alpha
    }

    public void SetQuality(int qualityIndex)
    {
        _qualityLevel = qualityIndex;
    }

    public void GraphicsApply()
    {
        PlayerPrefs.SetFloat("masterBrightness", _brightnessLevel);
        // Change your brightness with your post processing or whatever it is

        PlayerPrefs.SetInt("masterQuality", _qualityLevel);
        QualitySettings.SetQualityLevel(_qualityLevel);

        StartCoroutine(ConfirmationBox());
    }

    #endregion

    #region Sound Settings

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        volumeTextValue.text = volume.ToString("0.0");
    }

    public void VolumeApply()
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

    public void GameplayApply()
    {
        PlayerPrefs.SetFloat("masterSensitivity", mainControllerSen);
        PlayerPrefs.SetFloat("masterAimSensitivity", aimControllerSen);
        PlayerPrefs.SetInt("masterInvertY", invertYToggle.isOn ? -1 : 1);
        StartCoroutine(ConfirmationBox());
    }

    #endregion

    #endregion

    #region Reset And Exit

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

    public void ExitGameButton()
    {
        Application.Quit();
    }

    #endregion

    #region Methods

    private void UpdateBrightness(float newBrightness)
    {
        _brightnessLevel = newBrightness;
        brightnessSlider.value = newBrightness;
        brightnessTextValue.text = newBrightness.ToString("0.0");
        brightnessPanel.color = new Color(brightnessPanel.color.r, // Red
                                          brightnessPanel.color.g, // Green
                                          brightnessPanel.color.b, // Blue
                                          1 - newBrightness); // Alpha
    }

    private void UpdateQuality(int newQuality)
    {
        _qualityLevel = newQuality;
        qualityDropdown.value = newQuality;
        QualitySettings.SetQualityLevel(newQuality);
    }

    private void UpdateVolume(float newVolume)
    {
        AudioListener.volume = newVolume;
        volumeSlider.value = newVolume;
        volumeTextValue.text = newVolume.ToString("0.0");
    }

    private void UpdateControllerSensitivity(float newSensitivity)
    {
        sensitivityTextValue.text = newSensitivity.ToString("0.0");
        sensitivitySlider.value = newSensitivity;
        mainControllerSen = newSensitivity;
    }

    private void UpdateAimSensitivity(float newAimSensitivity)
    {
        sensitivityAimTextValue.text = newAimSensitivity.ToString("0.0");
        sensitivityAimSlider.value = newAimSensitivity;
        aimControllerSen = newAimSensitivity;
    }

    private void UpdateInvertY(bool newInvertY)
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
