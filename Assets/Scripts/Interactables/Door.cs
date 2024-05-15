using System;
using UnityEngine;

public class Door : InteractableBase
{
    [SerializeField] private float speed;
    [SerializeField] private Vector3 displacementUnits;
    [SerializeField] private bool canInteract = false;

    private bool activated;
    private bool interacting = false;
    private Vector3 destinationPosition;

    void Update()
    {
        if (interacting)
        {
            transform.position = Vector3.MoveTowards(transform.position, destinationPosition, speed * Time.deltaTime);
            if ((Vector3.Distance(transform.position, destinationPosition) < 0.1))
            {
                interacting = false;
            }
        }
    }

    public override bool CanInteract(PowerType powerType)
    {
        return canInteract;
    }

    public override void Interact()
    {
        if (Activated())
        {
            destinationPosition = transform.position - displacementUnits;
            activated = false;
        }
        else
        {
            destinationPosition = transform.position + displacementUnits;
            activated = true;
        }
        interacting = true;


    }

    public override bool Activated()
    {
        return activated;
    }
}
