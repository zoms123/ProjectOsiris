using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private string message;
    [SerializeField] private Texture icon;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private RawImage tutorialIcon;
    [SerializeField] private GameObject tutorial;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tutorialText.text = message;
            tutorialIcon.texture = icon;
            tutorial.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tutorial.gameObject.SetActive(false);
        }
    }
}
