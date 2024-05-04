using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestColumnInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject zeroGravityZone;
    [SerializeField] private bool active;
    [SerializeField] private float attachingMovementSpeedFactor = 3;
    [SerializeField] private Transform detachPoint;
    private IInteractable interactable;
    private IAttachable attachable;
    private GravityMovableObject gravityMovableObject;

    public event Action OnLoseObject;

    private void Start()
    {
        gravityMovableObject = GetComponentInChildren<GravityMovableObject>();
        
        
        if (gravityMovableObject != null)
        {
            interactable = gravityMovableObject.GetComponent<IInteractable>();
            attachable = gravityMovableObject.GetComponent<IAttachable>();
            Interact();
        }
    }

    public bool CanInteract(PowerType powerType)
    {
        return true;
    }

    public void Interact()
    {
        if(interactable != null && !active)
        {
            gravityMovableObject.MultiplySpeed(attachingMovementSpeedFactor);
            attachable.ChangeParent(zeroGravityZone.transform);
            interactable.Interact();
            active = true;
        } else if ( active && gravityMovableObject.Attached)
        {
            attachable.ChangeParent(detachPoint);
            if (interactable.Activated())
            {
                interactable.Interact();
            }
            interactable.Interact();
            gravityMovableObject.DeactivateWhenAttached();
            active = false;
        }
    }

    public bool Activated()
    {
        return active;
    }

    #region Collisions and Triggers

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IInteractable>() != null && interactable == null)
        {
            interactable = other.GetComponent<IInteractable>();
            attachable = other.GetComponent<IAttachable>();
            gravityMovableObject = other.GetComponent<GravityMovableObject>();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(interactable == null && other.GetComponent<IInteractable>() != null)
        {
            interactable = other.GetComponent<IInteractable>();
            attachable = other.GetComponent<IAttachable>();
            gravityMovableObject = other.GetComponent<GravityMovableObject>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<IInteractable>() == interactable)
        {
            Debug.Log("Exit trigger");
            interactable = null;
            attachable = null;
            gravityMovableObject = null;
        }
    }

    #endregion

}
