using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityMovableObject : MonoBehaviour, IInteractable
{
    [SerializeField] private float overlapSphereRadius;
    [SerializeField] private float attachingMovementSpeed;

    private Rigidbody rigidBody;
    private ZeroGravityEffector zeroGravityEffector;
    private Vector3 attachPointPosition;
    private bool activated;
    private bool attached;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {/*
        if(activated && !attached)
        {
            if(transform.position.y != attachPointPosition.y)
            {
                Vector3 targetPosition = new Vector3(transform.position.x, attachPointPosition.y, transform.position.z);
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, 0);
            } else if(transform.position.x != attachPointPosition.x || transform.position.z != attachPointPosition.z)
            {
                Vector3 targetPosition = new Vector3(attachPointPosition.x, transform.position.y, attachPointPosition.z);
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, 0);
            } else if(transform.position == attachPointPosition)
            {
                attached = true;
                zeroGravityEffector.UseZeroGravity();
            }

        }*/
    }

    private void FixedUpdate()
    {
        if (activated && !attached)
        {
            if (transform.position.y != attachPointPosition.y)
            {
                Vector3 targetPosition = new Vector3(transform.position.x, attachPointPosition.y, transform.position.z);
                Vector3 diretion = (targetPosition - transform.position).normalized;
                Vector3 newPosition = transform.position + diretion * attachingMovementSpeed * Time.deltaTime;
                rigidBody.MovePosition(newPosition);

            }
            else if (transform.position.x != attachPointPosition.x || transform.position.z != attachPointPosition.z)
            {
                Vector3 targetPosition = new Vector3(attachPointPosition.x, transform.position.y, attachPointPosition.z);
                Vector3 diretion = (targetPosition - transform.position).normalized;
                Vector3 newPosition = transform.position + diretion * attachingMovementSpeed * Time.deltaTime;
                rigidBody.MovePosition(newPosition);
            }
            else if (transform.position == attachPointPosition)
            {
                attached = true;
                zeroGravityEffector.UseZeroGravity();
            }

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
                    transform.SetParent(collider.transform);
                    attachPointPosition = collider.transform.Find("AttachPoint").position;
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
