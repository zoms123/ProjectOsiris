using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private static bool gameStarted;
    private AsyncOperation asyncOperation;

    private void Awake()
    {
        gameStarted = false;
    }

    private IEnumerator StartGame()
    {
        Debug.Log("StartGame");
        while (asyncOperation != null && !asyncOperation.isDone)
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
           AsyncOperation asyncOperation = SceneManager.GetSceneByName(gameObject.name).isLoaded ? null : SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);

            if(!gameStarted)
            {
                FindFirstObjectByType<InputMovementSystem>().enabled = false;
                StartCoroutine(nameof(StartGame));
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
