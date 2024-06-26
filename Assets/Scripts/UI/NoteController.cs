using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NoteController : MonoBehaviour
{
	[SerializeField][TextArea] private string noteText;
	[SerializeField] private GameObject noteCanvas;
	[SerializeField] private TMP_Text noteTextUi;

	private bool playerInRange = false;

	// Update is called once per frame
	void Update()
	{
		if (playerInRange)
		{
			if (Input.GetKeyDown(KeyCode.E))
			{
				//Open Note
				noteTextUi.text = noteText;
				noteCanvas.SetActive(true);
			}
		}

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
		noteCanvas.SetActive(false);
	}
}
