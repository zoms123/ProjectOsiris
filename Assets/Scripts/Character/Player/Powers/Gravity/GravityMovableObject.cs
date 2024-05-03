using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityMovableObject : MonoBehaviour, IInteractable, IAttachable
{
    [SerializeField] private float overlapSphereRadius;
    [SerializeField] private float attachingMovementSpeed;

    public event Action OnLoseObject;

    private Rigidbody rigidBody;
    private ZeroGravityEffector zeroGravityEffector;
    private bool effectorActivated;
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
            rigidBody.velocity = Vector3.zero;
            rigidBody.AddForce(direction * attachingMovementSpeed, ForceMode.Impulse);

            if (Vector3.Distance(transform.position, transform.parent.position) < 0.1f)
            {
                rigidBody.velocity = Vector3.zero;
                transform.rotation = transform.parent.rotation;
                attached = true;
            }

        } else if (attached && !effectorActivated)
        {
            rigidBody.Sleep();
            zeroGravityEffector.UseZeroGravity();
            effectorActivated = true;
        }
        if (attached && effectorActivated && Vector3.Distance(transform.position, transform.parent.position) > 1f)
        {
            zeroGravityEffector.StopUsingZeroGravity();
            OnLoseObject?.Invoke();
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
            // TODO reorganize the code in order to: 
                // TODO 1: remove the dependency with Player, this way we should not use OverlapSphere to detect player
                // TODO 2: Create a method to set parent, once the parent change the status change to activated = false
            /*Collider[] collidersTouched = Physics.OverlapSphere(transform.position, overlapSphereRadius);
            foreach (Collider collider in collidersTouched)
            {
                if (collider.CompareTag("Player"))
                {
                    zeroGravityEffector = GetComponent<ZeroGravityEffector>();
                    transform.SetParent(collider.transform.Find("AttachPoint"));
                    activated = true;
                    rigidBody.velocity = Vector3.zero;
                    rigidBody.useGravity = false;
                    break;
                }
            }
            */
            activated = true;
            rigidBody.velocity = Vector3.zero;
            rigidBody.useGravity = false;
        }
        else
        {
            zeroGravityEffector.StopUsingZeroGravity();
            //transform.SetParent(null);
            attached = false;
            activated = false;
            effectorActivated = false;
        }
    }

    public bool Activated()
    {
        return activated;
    }

    #endregion

    #region IAttachable Interface implementation
    public void ChangeParent(Transform parentTransform)
    {
        if(parentTransform != transform.parent)
        {
            OnLoseObject?.Invoke();
            transform.parent = parentTransform;
        }
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
