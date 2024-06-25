using System;
using System.Collections;
using UnityEngine;

public class QuestColumnInteractable : InteractableBase, IActivable
{
    [SerializeField] private GameObject zeroGravityZone;
    [SerializeField] private bool active;
    [SerializeField] private float attachingMovementSpeedFactor = 3;
    [SerializeField] private Transform detachPoint;
    private IInteractable interactable;
    private IAttachable attachable;
    private GravityMovableObject gravityMovableObject;

    public event Action OnActivated;
    public event Action OnDeactivated;

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

    public override bool CanInteract(PowerType powerType)
    {
        return true;
    }

    public override void Interact()
    {
        if(interactable != null && !active)
        {
            gravityMovableObject.MultiplySpeed(attachingMovementSpeedFactor);
            attachable.ChangeParent(zeroGravityZone.transform);
            interactable.Interact();
            StartCoroutine(ChangetToActiveWhenAttached());
            
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
            // TODO Change color here
            OnDeactivated?.Invoke();
        }
    }

    public override bool Activated()
    {
        return active;
    }

    #region Coroutines
    private IEnumerator ChangetToActiveWhenAttached()
    {
        while (!attachable.Attached)
        {
            yield return new WaitForSeconds(0.5f);
        }
        active = true;
        // TODO Change color here
        OnActivated?.Invoke();
        
    }
    #endregion

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
            interactable = null;
            attachable = null;
            gravityMovableObject = null;
        }
    }

    #endregion

}
