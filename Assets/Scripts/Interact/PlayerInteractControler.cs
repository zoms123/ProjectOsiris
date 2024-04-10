using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class PlayerInteractControler : MonoBehaviour
{
    [SerializeField] private Transform point;
    [SerializeField] private float radius;

    [SerializeField] private InputManagerSO inputManager;

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
