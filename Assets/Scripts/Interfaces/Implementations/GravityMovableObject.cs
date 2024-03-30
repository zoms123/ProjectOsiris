using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityMovableObject : MonoBehaviour, IInteractable
{

    private ZeroGravityEffector zeroGravityEffector;
    private bool activated;
    private Transform parent; 
    public bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {
        if (!activated)
        {
            zeroGravityEffector = GetComponent<ZeroGravityEffector>();
            zeroGravityEffector.UseZeroGravity();
            transform.parent = parent;
            activated = true;
        }
        else
        {
            zeroGravityEffector.StopUsingZeroGravity();
            transform.parent = null;
            activated = false;
        }
        
    }


    #region Collisions and Triggers
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            parent = other.transform;
        }
    }
    #endregion
}
