using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Managers/GameManager")]
public class GameManagerSO : ScriptableObject
{
    [SerializeField] private InputManagerSO inputManager;
    [SerializeField] private PowerType currentPowerType = PowerType.None;
    public PowerType CurrentPowerType { get { return currentPowerType; } }

    public  InputManagerSO InputManager { get { return inputManager; } }


    private void OnEnable()
    {
        inputManager.OnPowerSelect += OnPowerSelected;
    }

    private void OnDisable()
    {
        inputManager.OnPowerSelect -= OnPowerSelected;
    }

    private void OnPowerSelected(Vector2 power)
    {   if (power[1] == -1)
        {
            currentPowerType = PowerType.Gravity;

        }
        else if (power[0] == -1)
        {
            currentPowerType = PowerType.Crystal;
        }
        else if (power[0] == 1)
        {
            currentPowerType = PowerType.Time;
        }
        else
        {
            currentPowerType = PowerType.Shadow;
        }
    }

}
