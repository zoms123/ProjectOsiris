using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Required] private GameManagerSO gameManager;

    [Header("Canvas Elemets")]
    [SerializeField] private RawImage powerIcon;
    [SerializeField] private RawImage lowLifeImage;
    [SerializeField] private GameObject noteUI;
    [SerializeField] private TMP_Text noteTextUi;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private RawImage tutorialIcon;
    [SerializeField] private GameObject tutorial;

    [Header("Textures")]
    [SerializeField] private Texture2D gravity;
    [SerializeField] private Texture2D crystal;
    [SerializeField] private Texture2D time;
    [SerializeField] private Texture2D shadow;
    [SerializeField] private Texture2D none;

    #region Events

    private void OnEnable()
    {
        gameManager.OnPlayerChangePower += ChangePowerUI;
        gameManager.OnPlayerLowLife += ShowLowLifeUI;
        gameManager.OnPlayerRestoreLife += HideLowLifeUI;
        gameManager.OnPlayerOpenNote += ShowNoteUI;
        gameManager.OnPlayerCloseNote += HideNoteUI;
        gameManager.OnPlayerEnterTutorialZone += ShowTutorialUI;
        gameManager.OnPlayerExitTutorialZone += HideTutorialUI;
    }

    private void ChangePowerUI(PowerType powerType)
    {
        switch (powerType)
        {
            case PowerType.Gravity:
                powerIcon.texture = gravity;
                break;

            case PowerType.Crystal:
                powerIcon.texture = crystal;
                break;

            case PowerType.Time:
                powerIcon.texture = time;
                break;

            case PowerType.Shadow:
                powerIcon.texture = shadow;
                break;

            case PowerType.None:
                powerIcon.texture = none;
                break;
        }
    }

    private void ShowLowLifeUI()
    {
        lowLifeImage.gameObject.SetActive(true);
    }

    private void HideLowLifeUI()
    {
        lowLifeImage.gameObject.SetActive(false);
    }

    private void ShowNoteUI(string noteText)
    {
        noteTextUi.text = noteText;
        noteUI.SetActive(true);
    }

    private void HideNoteUI()
    {
        noteUI.SetActive(false);
    }

    private void ShowTutorialUI(string message, Texture icon)
    {
        tutorialText.text = message;
        tutorialIcon.texture = icon;
        tutorial.gameObject.SetActive(true);
    }

    private void HideTutorialUI()
    {
        tutorial.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        gameManager.OnPlayerChangePower -= ChangePowerUI;
        gameManager.OnPlayerLowLife -= ShowLowLifeUI;
        gameManager.OnPlayerRestoreLife -= HideLowLifeUI;
        gameManager.OnPlayerOpenNote -= ShowNoteUI;
        gameManager.OnPlayerCloseNote -= HideNoteUI;
        gameManager.OnPlayerEnterTutorialZone -= ShowTutorialUI;
        gameManager.OnPlayerExitTutorialZone -= HideTutorialUI;
    }

    #endregion
}
