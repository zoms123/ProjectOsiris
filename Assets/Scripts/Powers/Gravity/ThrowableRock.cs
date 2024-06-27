using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ThrowableRock : DistanceAttack
{
    public override void Initialize(Vector3 direction, GameObject ownerObject)
    {
        base.Initialize(direction, ownerObject);
    }
    protected override void PerformAttack()
    {
          transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    protected void ReturnToPool()
    {
        initialized = false;
        ObjectPooler.Instance.Despawn(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject != OwnerObject)
        {
            Debug.Log("Hit " + other.name);
            ReturnToPool();
        }
    }
}
