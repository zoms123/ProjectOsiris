using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityMovableObject : MonoBehaviour, IInteractable
{
    [SerializeField] private float overlapSphereRadius;

    private ZeroGravityEffector zeroGravityEffector;
    private bool activated;

    public bool CanInteract(PowerType powerType)
    {
        return powerType == PowerType.Gravity;
    }

    public void Interact()
    {
        if (!activated)
        {
            
            Collider[] collidersTouched = Physics.OverlapSphere(transform.position, overlapSphereRadius);
            foreach (Collider collider in collidersTouched)
            {
                if (collider.CompareTag("Player"))
                {
                    zeroGravityEffector = GetComponent<ZeroGravityEffector>();
                    zeroGravityEffector.UseZeroGravity();
                    transform.SetParent(collider.transform);
                    activated = true;
                    break;
                }
            }
        }
        else
        {
            zeroGravityEffector.StopUsingZeroGravity();
            transform.SetParent(null);
            activated = false;
        }
    }

    public bool Activated()
    {
        return activated;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, overlapSphereRadius);
    }
}
