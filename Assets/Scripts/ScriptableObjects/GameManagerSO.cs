using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Managers/GameManager")]
public class GameManagerSO : ScriptableObject
{
    [Header("References")]
    [SerializeField] private InputManagerSO inputManager;
    [SerializeField] private PowerType currentPowerType = PowerType.None;
    //[SerializeField] private RawImage powerIcon;

    [Header("Textures")]
    [SerializeField] private Texture2D gravity;
    [SerializeField] private Texture2D crystal;
    [SerializeField] private Texture2D time;
    [SerializeField] private Texture2D shadow;

    public PowerType CurrentPowerType { get { return currentPowerType; } }
    public  InputManagerSO InputManager { get { return inputManager; } }


    /*private void OnEnable()
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
            powerIcon.texture = gravity;
        }
        else if (power[0] == -1)
        {
            currentPowerType = PowerType.Crystal;
            powerIcon.texture = crystal;
        }
        else if (power[0] == 1)
        {
            currentPowerType = PowerType.Time;
            powerIcon.texture = time;
        }
        else
        {
            currentPowerType = PowerType.Shadow;
            powerIcon.texture = shadow;
        }
    }*/

}
