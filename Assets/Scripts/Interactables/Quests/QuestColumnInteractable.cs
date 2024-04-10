using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestColumnInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject zeroGravityZone;
    [SerializeField] private bool active;
    private GameObject interactable;
    


    public bool CanInteract(PowerType powerType)
    {
        return true;
    }

    public void Interact()
    {
        if(interactable != null)
        {
            interactable.transform.parent = zeroGravityZone.transform;
            interactable.transform.localPosition = Vector3.zero;
            active = true;
        }
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
