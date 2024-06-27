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
    public event Action<string, Texture> OnPlayerEnterTutorialZone;
    public event Action OnPlayerExitTutorialZone;

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

    public void PlayerEnterTutorialZone(string message, Texture icon)
    {
        OnPlayerEnterTutorialZone?.Invoke(message, icon);
    }

    public void PlayerExitTutorialZone()
    {
        OnPlayerExitTutorialZone?.Invoke();
    }

    #endregion
}
