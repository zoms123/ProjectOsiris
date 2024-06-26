using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Required] private GameManagerSO gameManager;

    [Header("Canvas Elemets")]
    [SerializeField] private RawImage powerIcon;
    [SerializeField] protected RawImage lowLifeImage;

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

    private void OnDisable()
    {
        gameManager.OnPlayerChangePower -= ChangePowerUI;
        gameManager.OnPlayerLowLife -= ShowLowLifeUI;
        gameManager.OnPlayerRestoreLife -= HideLowLifeUI;
    }

    #endregion
}
