using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private InputManagerSO inputManager;
    [SerializeField] private GameObject gameInterface;
    [SerializeField] private GameObject optionsInterface;

    private void OnEnable()
    {
        inputManager.OnOptions += Pause;
    }

    private void OnDisable()
    {
        inputManager.OnOptions -= Pause;
    }

    private void Pause()
    {
        if (Time.timeScale != 0f)
        {
            Time.timeScale = 0f;
            gameInterface.SetActive(false);
            optionsInterface.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            optionsInterface.SetActive(false);
            gameInterface.SetActive(true);
        }
    }
}
