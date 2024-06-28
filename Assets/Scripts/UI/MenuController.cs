using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MenuController : OptionsSettings
{
    [Header("Levels To Load")]
    [SerializeField, Required] private GameObject noSavedGameDialog = null;
    [SerializeField] private string _newGameLevel;
    private string levelToLoad;

    [Header("Canvas Elements")]
    [SerializeField, Required] private Image brightnessPanel;

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

    #region Public Methods

    public void UpdateInteractableSelected(GameObject newSelection)
    {
        EventSystem eventSystem = EventSystem.current;
        eventSystem.SetSelectedGameObject(newSelection, new BaseEventData(eventSystem));
    }

    public void ExitGameButton()
    {
        Application.Quit();
    }

    #endregion

    #region Methods

    private void UpdateBrightnessPanel(float newBrightness)
    {
        brightnessPanel.color = new Color(brightnessPanel.color.r, // Red
                                          brightnessPanel.color.g, // Green
                                          brightnessPanel.color.b, // Blue
                                          1 - newBrightness); // Alpha
    }

    #endregion

    #region Override Methods

    public override void SetBrightness(float brightness)
    {
        base.SetBrightness(brightness);
        UpdateBrightnessPanel(brightness);
    }

    public override void GraphicsApply()
    {
        base.GraphicsApply();
        UpdateBrightnessPanel(_brightnessLevel);
    }

    public override void VolumeApply()
    {
        base.VolumeApply();
        // Pendiente actualizacion
    }

    #endregion
}
