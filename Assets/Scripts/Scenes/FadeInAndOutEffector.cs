using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class FadeInAndOutEffector : MonoBehaviour
{
    private Animator animator;
    private bool activated;

    private void OnDisable()
    {
        
    }


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void FadeOut()
    {
        animator.SetTrigger("FadeOut");
        Invoke(nameof(DisableObject), 1f); 
    }

    public void DisableObject()
    {
        gameObject.SetActive(false);
    }
}
