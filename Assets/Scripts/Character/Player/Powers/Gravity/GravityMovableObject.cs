using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityMovableObject : MonoBehaviour, IInteractable
{
    [SerializeField] private float overlapSphereRadius;
    [SerializeField] private float attachingMovementSpeed;

    private Rigidbody rigidBody;
    private ZeroGravityEffector zeroGravityEffector;
    private bool effectorActivated;
    private Vector3 attachPointPosition;
    private bool activated;
    private bool attached;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (activated && !attached) 
        { 
            Vector3 direction = (transform.parent.position - transform.position).normalized;
            rigidBody.AddForce(direction * attachingMovementSpeed, ForceMode.VelocityChange);

            if (Vector3.Distance(transform.position, transform.parent.position) < 0.1f)
            {
                rigidBody.velocity = Vector3.zero;
                transform.rotation = transform.parent.rotation;
                attached = true;
            }

        } else if (attached && !effectorActivated)
        {
            zeroGravityEffector.UseZeroGravity();
            effectorActivated = true;
        }

        
    }

    #region IInteractable Interface implementation
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
                    transform.SetParent(collider.transform.Find("AttachPoint"));
                    activated = true;
                    rigidBody.useGravity = false;
                    break;
                }
            }
        }
        else
        {
            zeroGravityEffector.StopUsingZeroGravity();
            transform.SetParent(null);
            attached = false;
            activated = false;
        }
    }

    public bool Activated()
    {
        return activated;
    }

    #endregion

    #region Debug

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, overlapSphereRadius);
    }

    #endregion
}
