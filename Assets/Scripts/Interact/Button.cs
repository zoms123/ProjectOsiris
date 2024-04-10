using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour, IInteractable
{
    [SerializeField] private float speed;
    [SerializeField] private Vector3 displacementUnits;
    [SerializeField] private GameObject objectToMove;

    private bool canInteract = true;
    private bool interacting = false;
    private Vector3 destinationPosition;

    void Update()
    {
        if (interacting)
        {
            objectToMove.transform.position = Vector3.MoveTowards(objectToMove.transform.position, destinationPosition, speed * Time.deltaTime);
            if ((Vector3.Distance(objectToMove.transform.position, destinationPosition) < 0.1))
            {
                interacting = false;
            }
        }
    }

    public bool CanInteract()
    {
        return canInteract;
    }

    public void Interact(PowerType powerType)
    {
        destinationPosition = objectToMove.transform.position + displacementUnits;
        interacting = true;
        canInteract = false;
    }
}
