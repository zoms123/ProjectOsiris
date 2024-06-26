using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class GravityMovableObject : MonoBehaviour, IInteractable, IAttachable, IMovable, ILosableObject
{
    [SerializeField] private float overlapSphereRadius;
    [SerializeField] private float attachingMovementSpeed;
    [SerializeField] private bool attached;

    public event Action OnLoseObject;

    private Rigidbody rigidBody;
    private ZeroGravityEffector zeroGravityEffector;
    private bool effectorActivated;
    private bool activated;
    
    private float originalSpeed;

    public bool Attached { get { return attached; } }
    

    private void Awake()
    {
        originalSpeed = attachingMovementSpeed;
        rigidBody = GetComponent<Rigidbody>();
        zeroGravityEffector = GetComponent<ZeroGravityEffector>();
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
        if (attached && effectorActivated && !CheckIfInteractableIsInRange())
        {
            OnLoseObject?.Invoke();
        }
    }

    private bool CheckIfInteractableIsInRange()
    {
        return transform.position.y >= transform.parent.position.y - 0.5f
            && transform.localPosition.z >= -1.5f
            && transform.position.y <= transform.parent.position.y + 5f
            && transform.localPosition.z <= 5f;
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
            activated = true;
            rigidBody.velocity = Vector3.zero;
            rigidBody.useGravity = false;
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            zeroGravityEffector.StopUsingZeroGravity();
            attached = false;
            activated = false;
            effectorActivated = false;
            transform.GetChild(0).gameObject.SetActive(false);
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
        if (parentTransform != transform.parent)
        {
            //OnLoseObject?.Invoke();
            transform.parent = parentTransform;
        }
    }
    #endregion

    #region IMovable Interface implementation
    public Rigidbody GetRigidbody()
    {
        return rigidBody;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public Vector3 GetLocalPosition()
    {
        return transform.localPosition;
    }


    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void SetLocalPosition(Vector3 localPosition)
    {
        transform.localPosition = localPosition;
    }
    #endregion

    public void MultiplySpeed(float factor) 
    {
        attachingMovementSpeed *= factor;
    }

    public void ResetSpeed()
    {
        attachingMovementSpeed = originalSpeed;
    }

    public void DeactivateWhenAttached()
    {
        StartCoroutine(DeactivateWhenAttachedRoutine());
    }


    private IEnumerator DeactivateWhenAttachedRoutine()
    {
        while (!attached)
        {
            yield return new WaitForSeconds(1);
        }
        ChangeParent(null);
        ResetSpeed();
        Interact();
    }
    #region Debug

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, overlapSphereRadius);
    }

    #endregion
}