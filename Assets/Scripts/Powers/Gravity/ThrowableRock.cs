using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ThrowableRock : DistanceAttack
{
    public override void Initialize(Vector3 direction, string ownerTag)
    {
        base.Initialize(direction, ownerTag);
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
        if (!other.CompareTag(OwnerTag))
        {
            Debug.Log("Hit " + other.name);
            ReturnToPool();
        }
    }
}
