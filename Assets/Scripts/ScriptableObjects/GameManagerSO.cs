using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Managers/GameManager")]
public class GameManagerSO : ScriptableObject
{
    // UI Powers
    public event Action<PowerType> OnPlayerChangePower;

    // UI Life
    public event Action OnPlayerLowLife;
    public event Action OnPlayerRestoreLife;

    // UI Notes
    public event Action<string> OnPlayerOpenNote;
    public event Action OnPlayerCloseNote;

    // UI Tutorial
    public event Action<string, Texture, Texture> OnPlayerEnterTutorialZone;
    public event Action OnPlayerExitTutorialZone;

    // UI PauseMenu
    public event Action<float> OnUpdateBrightness;
    public event Action OnUpdateControllerSensitivity;
    public event Action OnUpdateAimSensitivity;
    public event Action OnUpdateInvertY;

    #region Public Methods

    public void PlayerChangePower(PowerType powerType)
    {
        OnPlayerChangePower?.Invoke(powerType);
    }

    public void PlayerLowLife()
    {
        OnPlayerLowLife?.Invoke();
    }

    public void PlayerRestoreLife()
    {
        OnPlayerRestoreLife?.Invoke();
    }

    public void PlayerOpenNote(string noteText)
    {
        OnPlayerOpenNote?.Invoke(noteText);
    }

    public void PlayerCloseNote()
    {
        OnPlayerCloseNote?.Invoke();
    }

    public void PlayerEnterTutorialZone(string message, Texture iconPC, Texture iconPS5)
    {
        OnPlayerEnterTutorialZone?.Invoke(message, iconPC, iconPS5);
    }

    public void PlayerExitTutorialZone()
    {
        OnPlayerExitTutorialZone?.Invoke();
    }

    public void UpdateBrightness(float newBrightness)
    {
        OnUpdateBrightness?.Invoke(newBrightness);
    }

    public void UpdateControllerSensitivity()
    {
        OnUpdateControllerSensitivity?.Invoke();
    }

    public void UpdateAimSensitivity()
    {
        OnUpdateAimSensitivity?.Invoke();
    }

    public void UpdateInvertY()
    {
        OnUpdateInvertY?.Invoke();
    }


    #endregion
}
