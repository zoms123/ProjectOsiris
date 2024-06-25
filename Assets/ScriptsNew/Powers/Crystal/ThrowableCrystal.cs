using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableCrystal : DistanceAttack
{
    // called on Update if initialized
    protected override void PerformAttack()
    {
        // move in the firing direction
        transform.Translate(speed * Time.deltaTime * direction, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log($"Hit {other.name}");
            ObjectPooler.Instance.Despawn(gameObject);
        }
        else // hit other object
        {
            Debug.Log($"Hit {other.name}");
            ObjectPooler.Instance.Despawn(gameObject);
        }
    }
}
