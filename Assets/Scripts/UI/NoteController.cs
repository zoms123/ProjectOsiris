using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NoteController : MonoBehaviour, IInteractable
{
	[SerializeField][TextArea] private string noteText;
	[SerializeField] private GameObject noteUI;
	[SerializeField] private TMP_Text noteTextUi;

	private bool playerInRange = false;

    public bool CanInteract(PowerType powerType)
    {
		return playerInRange;
    }

    public void Interact()
    {
        if (!Activated())
        {
            //Open Note
            noteTextUi.text = noteText;
            noteUI.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            //Close Note
            noteUI.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    public bool Activated()
    {
		return noteUI.activeSelf;
    }

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
        noteUI.SetActive(false);
    }
}
