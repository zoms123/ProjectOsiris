using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private static bool gameStarted;

    private void Awake()
    {
        gameStarted = false;
    }

    private IEnumerator StartGame(AsyncOperation asyncOperation)
    {
        Debug.Log("StartGame");
        while (!asyncOperation.isDone)
        {
            Debug.Log("Waiting"); 
            yield return new WaitForSeconds(0.1f);
        }


        yield return new WaitForSeconds(1.15f);
        FindFirstObjectByType<FadeInAndOutEffector>().FadeOut();
        Debug.Log("Reset time");
        gameStarted = true;
        FindFirstObjectByType<InputMovementSystem>().enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AsyncOperation asyncOperation =  SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            
            Debug.Log("Current Time " + Time.timeScale);
            if(!gameStarted)
            {
                FindFirstObjectByType<InputMovementSystem>().enabled = false;
                StartCoroutine(nameof(StartGame), asyncOperation);
            }
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.UnloadSceneAsync(gameObject.name);
        }
    }
}
