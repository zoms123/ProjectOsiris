using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MeleeAttack : MonoBehaviour, IMeleeAttack
{
    protected bool initialized;
    protected GameObject ownerObject;

    public GameObject OwnerObject { get { return ownerObject; } }

    public void Initialize(GameObject ownerObject)
    {
        gameObject.SetActive(true);
        initialized = true;
        this.ownerObject = ownerObject;
    }

    protected void Update()
    {
        if (initialized)
        {
            PerformAttack();
        }
    }

    protected abstract void PerformAttack();
}
