using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCombat : MonoBehaviour
{
    [Header("Attack System")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackDamage;
    [SerializeField] private float radius;
    [SerializeField] private LayerMask whatIsDamageable;
    [SerializeField] private string myTag;
    [SerializeField] private float timeBetweenAttacks;

    private IAttackStrategy attackStrategy;
    private PlayerDetector playerDetector;

    public float TimeBetweenAttacks { get { return timeBetweenAttacks; } }

    private void Awake()
    {
        playerDetector = GetComponent<PlayerDetector>();
        if(playerDetector)
            attackStrategy = new MeleAttackStrategy(attackPoint.position, radius, whatIsDamageable, myTag, attackDamage, playerDetector);
    }

    public void Attack()
    {
        attackStrategy.Execute();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(attackPoint.position, radius);
    }
}
