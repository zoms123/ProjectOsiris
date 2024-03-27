using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeroGravityZone : MonoBehaviour
{

    public Action disableZeroGravityEffect;
    private List<ZeroGravityEffector> zeroGravityEffectors;

    private void Awake()
    {
        zeroGravityEffectors = new List<ZeroGravityEffector>();
    }
    private void OnDisable()
    {
        foreach(ZeroGravityEffector effector in zeroGravityEffectors)
        {
            effector.StopUsingZeroGravity();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ZeroGravityEffector effector = other.GetComponent<ZeroGravityEffector>();
        if (effector)
        {
            zeroGravityEffectors.Add(effector);
            effector.UseZeroGravity();
        }
    }
}
