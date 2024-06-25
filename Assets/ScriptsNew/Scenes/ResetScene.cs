using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ResetScene : MonoBehaviour
{


    private void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}