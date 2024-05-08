using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeSystem : MonoBehaviour
{
    [Header("Lifes System")]
    [SerializeField] protected float health;
    [SerializeField] protected RawImage lowLifeImage;


    private void Update()
    {
        if (health <= 0)
        {
            lowLifeImage.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
        else
        {
            if (health <= 0)
            {
                yield return new WaitForSeconds(0.5f);
                Destroy(gameObject);
            }
        }
    }

    public void ReceiveDamage(float damageRecibed)
    {
        Debug.Log("Actor Damaged!!!!\nCurrent Heath: " + health + "\nDamage Recibed: " + damageRecibed + "\nNew Heath: " + (health - damageRecibed));
        health -= damageRecibed;
    }

    #region Collisions and Triggers

    
    
    private void OnCollisionEnter(Collision collision)
    {
        DamageDealer damageDealer = collision.gameObject.GetComponent<DamageDealer>();
        if (damageDealer)
        {
            ReceiveDamage(damageDealer.Damage);
        }
    }
    #endregion
}
