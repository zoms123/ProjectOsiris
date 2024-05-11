using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCombat : MonoBehaviour
{
    [Header("Attack System")]
    [SerializeField] private EAttackType attackType;
    [SerializeField] private GameObject attackPrefab;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackDamage;
    [SerializeField] private float radius;
    [SerializeField] private LayerMask whatIsDamageable;
    [SerializeField] private string myTag;
    [SerializeField] private float timeBetweenAttacks;

    private IAttackStrategy attackStrategy;
    private PlayerDetector playerDetector; 
    private float currentTime;
    private Animator animator;

    public float TimeBetweenAttacks { get { return timeBetweenAttacks; } }

    private void Awake()
    {
        playerDetector = GetComponent<PlayerDetector>();
        animator = GetComponent<Animator>();
        if (playerDetector)
        {
            switch (attackType)
            {
                case EAttackType.MELE: 
                    {
                        attackStrategy = new MeleAttackStrategy(attackDamage, playerDetector);
                        break;
                    }
                case EAttackType.DISTANCE:
                    {
                        attackStrategy = new DistanceAttackStrategy<ThrowableRock>(transform, animator, attackPrefab, attackPoint);
                        break;
                    }
            }
        }
            
    }

    public void Attack()
    {
        FocusTarget();
        attackStrategy.Execute();
    }

    private void FocusTarget()
    {
        Vector3 relativePos = playerDetector.Player.position - transform.position;
        relativePos.y = 0;
        transform.rotation = Quaternion.LookRotation(relativePos, Vector3.up);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(attackPoint.position, radius);
    }
}
