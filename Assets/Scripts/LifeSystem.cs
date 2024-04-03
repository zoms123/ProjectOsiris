using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeSystem : MonoBehaviour
{
    [Header("Lifes System")]
    [SerializeField] protected float health;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator ReceiveDamage(float damageRecibed)
    {
        Debug.Log("Actor Damaged!!!!\nCurrent Heath: " + health + "\nDamage Recibed: " + damageRecibed + "\nNew Heath: " + (health - damageRecibed));
        health -= damageRecibed;
        //Debug.Log("New Heath: " + health);
        if (health <= 0)
        {
            yield return new WaitForSeconds(0.5f);
            gameObject.SetActive(false);
        }
    }

    #region Collisions and Triggers

    
    
    private void OnCollisionEnter(Collision collision)
    {
        DamageDealer damageDealer = collision.gameObject.GetComponent<DamageDealer>();
        if (damageDealer)
        {
            StartCoroutine(ReceiveDamage(damageDealer.Damage));
        }
    }
    #endregion
}
