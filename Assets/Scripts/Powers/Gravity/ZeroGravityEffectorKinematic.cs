using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeroGravityEffectorKinematic : MonoBehaviour, IZeroGravityEffector
{
    [SerializeField] private float speed;
    [SerializeField] private Vector3 displacementUnits;

    private Vector3 initialPosition;
    private Vector3 destinationPosition;

    private bool useZeroGravity;

    public bool Activated { get { return useZeroGravity; } }

    private void Start()
    {
        initialPosition = transform.localPosition;
        destinationPosition = initialPosition + displacementUnits;
    }

    private void FixedUpdate()
    {
        if (useZeroGravity)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, destinationPosition, speed * Time.deltaTime);
        }
        else
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, initialPosition, speed * Time.deltaTime);
        }
    }

    public void UseZeroGravity()
    {
        useZeroGravity = true;
    }

    public void StopUsingZeroGravity()
    {
        useZeroGravity = false;
    }
}
