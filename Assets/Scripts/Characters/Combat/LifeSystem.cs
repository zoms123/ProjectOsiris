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
            if(lowLifeImage)
                lowLifeImage.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
        else if (CompareTag("Player") && health <= 20)
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


    private void OnTriggerEnter(Collider collider)
    {
        // TODO as we are using triggers deffine a way to differenciate who is shooting and who is receiving the damage
        DamageDealer damageDealer = collider.GetComponent<DamageDealer>();
        if (damageDealer && collider.GetComponent<DistanceAttack>().OwnerTag != tag)
        {
            ReceiveDamage(damageDealer.Damage);
        }
    }
    #endregion
}
