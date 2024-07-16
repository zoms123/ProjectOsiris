using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Required] private GameManagerSO gameManager;

    [Header("Tutorial Settings")]
    [SerializeField] private string message;
    [SerializeField] private Texture iconPC;
    [SerializeField] private Texture iconPS5;

    #region Collisions And Triggers

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.PlayerEnterTutorialZone(message, iconPC, iconPS5);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.PlayerExitTutorialZone();
        }
    }

    #endregion
}
