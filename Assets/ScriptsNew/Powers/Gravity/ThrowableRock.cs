using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableRock : DistanceAttack
{
    protected override void PerformAttack()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Hit " + other.name);
            ObjectPooler.Instance.Despawn(gameObject);
        }
    }
}
