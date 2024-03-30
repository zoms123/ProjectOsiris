using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    private float health;

    private void Start()
    {
        health = maxHealth;
    }

    private void Update()
    {
        // DEBUG
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(25f);
        }
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        health = Mathf.Max(0, health);

        if (health == 0) 
        {
            Debug.Log(gameObject.name + " is dead.");
        }
    }
}
