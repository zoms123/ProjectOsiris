using System.Collections;
using System.Collections.Generic;
using UnityEditor.Profiling;
using UnityEngine;
using UnityEngine.UI;

public class ChangePower : MonoBehaviour
{
    [SerializeField] private InputManagerSO inputManager;

    [SerializeField] private RawImage gravityIcon;
    [SerializeField] private RawImage crystalIcon;
    [SerializeField] private RawImage timeIcon;
    [SerializeField] private RawImage shadowIcon;

    private void OnEnable()
    {
        inputManager.OnPowerSelect += Change;
    }

    private void OnDisable()
    {
        inputManager.OnPowerSelect -= Change;
    }

    private void Change(Vector2 power)
    {
        gravityIcon.gameObject.SetActive(false);
        crystalIcon.gameObject.SetActive(false);
        timeIcon.gameObject.SetActive(false);
        shadowIcon.gameObject.SetActive(false);

        if (power[1] == -1)
        {
            gravityIcon.gameObject.SetActive(true);

        }
        else if (power[0] == -1)
        {
            crystalIcon.gameObject.SetActive(true);
        }
        else if (power[0] == 1)
        {
            timeIcon.gameObject.SetActive(true);
        }
        else
        {
            shadowIcon.gameObject.SetActive(true);
        }

    }
}
