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
}
