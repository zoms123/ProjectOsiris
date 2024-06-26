using System;
using UnityEngine;

public class RotableDoor : InteractableBase
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float rotationAngle;
    [SerializeField] private bool canInteract = false;

    private bool activated;
    private bool interacting = false;

    private float startAngle;
    private float targetAngle;
    private float elapsed;

    void Update()
    {
        if (interacting)
        {
            RotateDoor();
        }
    }

    public override bool CanInteract(PowerType powerType)
    {
        return canInteract;
    }

    public override void Interact()
    {

        if (interacting)
        {
            return;
        }

        activated = !activated;
        startAngle = transform.eulerAngles.y;
        targetAngle = startAngle + (activated ? rotationAngle : -rotationAngle);
        elapsed = 0f;
        interacting = true;


    }

    public override bool Activated()
    {
        return activated;
    }
    private void RotateDoor()
    {
        elapsed += Time.deltaTime;
        float duration = Mathf.Abs(rotationAngle) / rotationSpeed;
        float currentAngle = Mathf.Lerp(startAngle, targetAngle, elapsed / duration);

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, currentAngle, transform.eulerAngles.z);

        if (elapsed >= duration)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, targetAngle, transform.eulerAngles.z);
            interacting = false;
        }
    }
}
