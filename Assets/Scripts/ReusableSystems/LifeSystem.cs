using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Required] private GameManagerSO gameManager;

    [Header("Lifes System")]
    [SerializeField] private float maxHealth;

    private float health;
    private float timeLastAttack = 0f;

    public float Health { get { return health; } }
    public float MaxHealth { get { return maxHealth; } }
    public float TimeLastAttack { get { return timeLastAttack; } }

    private void Awake()
    {
        health = maxHealth;
    }

    private void Update()
    {
        timeLastAttack += Time.deltaTime;
    }

    #region Public Methods

    public void ReceiveDamage(float damageRecibed)
    {
        Debug.Log(tag + " Damaged!!!!\nCurrent Heath: " + health + "\nDamage Recibed: " + damageRecibed + "\nNew Heath: " + (health - damageRecibed));
        health -= damageRecibed;

        if (health <= 0f)
        {
            health = 0f;
            transform.GetComponent<Animator>().SetBool("Dead", true);
        }
        else if (CompareTag("Player") && health <= 20f)
        {
            gameManager.PlayerLowLife();
        } else
        {
            transform.GetComponent<Animator>().SetBool("Hit", true);
        }

        timeLastAttack = 0f;
    }

    public void RestoreLife(float lifeRestored)
    {
        health += lifeRestored;

        if (health > maxHealth)
        {
            health = maxHealth;
        }
        if (health > 20f)
        {
            gameManager.PlayerRestoreLife();
        }
    }

    #endregion

    #region Collisions and Triggers


    private void OnTriggerEnter(Collider collider)
    {
        // TODO as we are using triggers deffine a way to differenciate who is shooting and who is receiving the damage
        DamageDealer damageDealer = collider.GetComponent<DamageDealer>();
        if (damageDealer && collider.GetComponent<DistanceAttack>().OwnerObject.tag != tag)
        {
            ReceiveDamage(damageDealer.Damage);
        }
    }
    #endregion
}
