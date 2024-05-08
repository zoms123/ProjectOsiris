using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeSystem : MonoBehaviour
{
    [Header("Lifes System")]
    [SerializeField] protected float health;
    [SerializeField] protected RawImage lowLifeImage;

    public IEnumerator ReceiveDamage(float damageRecibed)
    {
        Debug.Log("Actor Damaged!!!!\nCurrent Heath: " + health + "\nDamage Recibed: " + damageRecibed + "\nNew Heath: " + (health - damageRecibed));
        health -= damageRecibed;
        
        if (tag == "Player")
        {
            if (health <= 0)
            {
                yield return new WaitForSeconds(0.5f);
                lowLifeImage.gameObject.SetActive(false);
                gameObject.SetActive(false);
            }
            else if (health <= 20)
            {
                lowLifeImage.gameObject.SetActive(true);
            }
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
