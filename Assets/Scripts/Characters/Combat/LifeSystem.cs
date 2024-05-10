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
        else if (tag == "Player" && health <= 20)
        {
            lowLifeImage.gameObject.SetActive(true);
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
