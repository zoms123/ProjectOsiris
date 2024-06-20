using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractController : MonoBehaviour
{
    [SerializeField, Required] private Transform point;
    [SerializeField] private float radius;

    [Header("Inputs")]

    [SerializeField, Required] private InputManagerSO inputManager;

    private void OnEnable()
    {
        inputManager.OnInteract += Interact;
    }

    private void OnDisable()
    {
        inputManager.OnInteract -= Interact;
    }

    public void Interact()
    {
        Collider[] collidersTouched = Physics.OverlapSphere(point.position, radius);
        foreach (Collider collider in collidersTouched)
        {
            IInteractable interactable = collider.GetComponent<IInteractable>();
            if (interactable != null && interactable.CanInteract(PowerType.None))
            {
                interactable.Interact();
                if (!interactable.Activated()) 
                {
                    interactable = null;
                }
                break;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(point.position, radius);
    }
}
