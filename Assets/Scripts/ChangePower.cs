using System.Collections;
using System.Collections.Generic;
using UnityEditor.Profiling;
using UnityEngine;
using UnityEngine.UI;

public class ChangePower : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManagerSO gameManager;
    [SerializeField] private InputManagerSO inputManager;
    [SerializeField] private RawImage powerIcon;

    [Header("Textures")]
    [SerializeField] private Texture2D gravity;
    [SerializeField] private Texture2D crystal;
    [SerializeField] private Texture2D time;
    [SerializeField] private Texture2D shadow;

    private void Change(Vector2 power)
    {
        switch (gameManager.CurrentPowerType)
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

            default:
                break;
        }
    }
}
