using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableCrystal : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float lifetime;
    private Vector3 direction;
    private bool initialized;

    public void Initialize(Vector3 direction)
    {
        this.direction = direction;
        initialized = true;
        Invoke("ReturnToPool", lifetime);
    }

    private void Update()
    {
        if (initialized)
        {
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
        }
    }

    private void ReturnToPool()
    {
        ObjectPool.ReturnObject(gameObject);
        initialized = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Hit " + other.name);
            ReturnToPool();
        }
    }
}
