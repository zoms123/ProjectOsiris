using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableCrystal : DistanceAttack
{
    protected override void PerformAttack()
    {
        // move in the firing direction
        transform.Translate(speed * Time.deltaTime * direction, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Hit " + other.name);
            DestroySelf();
        }
    }

    protected override void DestroySelf()
    {
        // reduce crystal lifetime and return it to the pool when its time reaches zero
        lifetime -= Time.deltaTime;
        if (lifetime < 0.0f) ObjectPooler.Instance.Despawn(gameObject);
    }
}
