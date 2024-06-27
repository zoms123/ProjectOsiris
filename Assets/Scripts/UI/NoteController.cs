using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NoteController : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField, Required] private GameManagerSO gameManager;

    [Header("Note Settings")]
    [SerializeField][TextArea] private string noteText;

	private bool playerInRange = false;
    private bool active = false;

    #region IInteractable

    public bool CanInteract(PowerType powerType)
    {
		return playerInRange;
    }

    public void Interact()
    {
        if (!Activated())
        {
            //Open Note
            gameManager.PlayerOpenNote(noteText);
            active = true;
            Time.timeScale = 0f;
        }
        else
        {
            //Close Note
            gameManager.PlayerCloseNote();
            active = false;
            Time.timeScale = 1f;
        }
    }

    public bool Activated()
    {
		return active;
    }

    #endregion

    #region Collisions And Triggers

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("jugador Dentro de rango");
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("jugador fuera de rango");
        playerInRange = false;
        gameManager.PlayerCloseNote();
        active = false;
    }

    #endregion
}
