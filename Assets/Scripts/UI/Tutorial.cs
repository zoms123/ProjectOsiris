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
    [SerializeField] private Texture icon;

    #region Collisions And Triggers

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.PlayerEnterTutorialZone(message, icon);
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
