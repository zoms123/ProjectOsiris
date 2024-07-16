using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenuManager : OptionsSettings
{
    [Header("References")]
    [SerializeField, Required] private InputManagerSO inputManager;
    [SerializeField, Required] private GameManagerSO gameManager;

    [Header("Pause Menu")]
    [SerializeField, Required] private GameObject gameInterface;
    [SerializeField, Required] private GameObject pauseMenuInterface;
    [SerializeField, Required] private GameObject resumeBtn;

    private string MAIN_MENU_LVL = "MainMenu";

    protected override void Awake()
    {
        base.Awake();

        inputManager.EnableGameplayInputs();
        Time.timeScale = 1f;
        pauseMenuInterface.SetActive(false);
        gameInterface.SetActive(true);
        UpdateInteractableSelected(null);
    }

    #region Events

    private void OnEnable()
    {
        inputManager.OnOptions += Pause;
    }

    private void Pause()
    {
        if (Time.timeScale != 0f)
        {
            inputManager.DisableGameplayInputs();
            Time.timeScale = 0f;
            gameInterface.SetActive(false);
            pauseMenuInterface.SetActive(true);
            UpdateInteractableSelected(resumeBtn);
        }
        else
        {
            inputManager.EnableGameplayInputs();
            Time.timeScale = 1f;
            pauseMenuInterface.SetActive(false);
            gameInterface.SetActive(true);
            UpdateInteractableSelected(null);
        }
    }

    private void OnDisable()
    {
        inputManager.OnOptions -= Pause;
    }

    #endregion

    #region Public Methods

    public void Unpause()
    {
        inputManager.EnableGameplayInputs();
        Time.timeScale = 1f;
        UpdateInteractableSelected(null);
    }

    public void UpdateInteractableSelected(GameObject newSelection)
    {
        EventSystem eventSystem = EventSystem.current;
        eventSystem.SetSelectedGameObject(newSelection, new BaseEventData(eventSystem));
    }

    public void ReturnMainMenu()
    {
        SceneManager.LoadScene(MAIN_MENU_LVL);
    }

    #endregion

    #region Override Methods

    public override void SetBrightness(float brightness)
    {
        base.SetBrightness(brightness);
        gameManager.UpdateBrightness(brightness);
    }

    public override void GraphicsApply()
    {
        base.GraphicsApply();
        gameManager.UpdateBrightness(_brightnessLevel);
    }

    public override void VolumeApply()
    {
        base.VolumeApply();
        // Pendiente actualizacion
    }

    public override void GameplayApply()
    {
        base.GameplayApply();
        gameManager.UpdateControllerSensitivity();
        gameManager.UpdateAimSensitivity();
        gameManager.UpdateInvertY();
    }

    #endregion
}
