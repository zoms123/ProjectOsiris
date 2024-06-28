using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class GroundCrystal : DistanceAttack
{
    public override void Initialize(Vector3 direction, GameObject ownerObject)
    {
        base.Initialize(direction, ownerObject);
        GetComponent<AudioSource>().Play();
    }

    protected void ReturnToPool()
    {
        initialized = false;
        ObjectPooler.Instance.Despawn(gameObject);
    }

    protected override void PerformAttack()
    {
        Invoke(nameof(ReturnToPool), lifetime);
    }
}
