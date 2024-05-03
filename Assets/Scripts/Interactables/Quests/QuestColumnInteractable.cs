using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestColumnInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject zeroGravityZone;
    [SerializeField] private bool active;
    private GameObject interactable;

    public event Action OnLoseObject;

    public bool CanInteract(PowerType powerType)
    {
        return true;
    }

    public void Interact()
    {
        if(interactable != null)
        {
            // TODO 3: when the method to change parent on interactable is included, call that method and right after call the Intereact method
            // TODO 4: create a script to verify if the interactable is now located at the zero position on in a range, if so, modify its position to zero and change to active = true

            interactable.transform.parent = zeroGravityZone.transform;
            interactable.transform.localPosition = Vector3.zero;
            active = true;
        }
    }

    public bool Activated()
    {
        return active;
    }

    #region Collisions and Triggers

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IInteractable>() != null)
        {
            interactable = other.gameObject;
        }
    }

    #endregion

}
