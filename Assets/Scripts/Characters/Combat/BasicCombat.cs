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
                case EAttackType.GRAVITY_BASIC:
                    attackStrategy = new BasicGravityAttackStrategy<ThrowableRock>(transform, animator, attackPrefab, attackPoint);
                    break;

                case EAttackType.CRYSTAL_BASIC:
                    attackStrategy = new BasicCrystalAttackStrategy<CrystalTrap>(transform, animator, attackPrefab, playerDetector);
                    break;

                case EAttackType.TIME_BASIC:
                    attackStrategy = new BasicTimeAttackStrategy<ThrowableRock>();
                    break;

                case EAttackType.SHADOW_BASIC:
                    attackStrategy = new BasicShadowAttackStrategy<ThrowableRock>();
                    break;

                default:
                    break;
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
