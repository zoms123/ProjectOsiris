using UnityEngine;

public class BasicCombat : MonoBehaviour
{
    [Header("Attack System")]
    [SerializeField] private EAttackType attackType;
    [SerializeField] private GameObject attackPrefab;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float attackDamageMultiplier = 1;
    [SerializeField] private float attackSpeedMultiplier = 1;
    [SerializeField] private float radius;
    [SerializeField] private LayerMask whatIsDamageable;
    [SerializeField] private string myTag;
    [SerializeField] private float timeBetweenAttacks;

    private IAttackStrategy attackStrategy;
    private PlayerDetector playerDetector; 
    private float currentTime;
    private Animator animator;

    public float TimeBetweenAttacks { get { return timeBetweenAttacks; } }
    public EAttackType AttackType { get { return attackType; } }

    public float AttackDamageMultiplier { get { return attackDamageMultiplier; } }
    public float AttackSpeedMultiplier { get { return attackSpeedMultiplier; } }


    private void Awake()
    {
        playerDetector = GetComponent<PlayerDetector>();
        animator = GetComponent<Animator>();
        if (playerDetector)
        {
            switch (attackType)
            {
                case EAttackType.GRAVITY_BASIC:
                    attackStrategy = new BasicGravityAttackStrategy<ThrowableRock>(transform, animator, attackPrefab, playerDetector, spawnPoint);
                    break;

                case EAttackType.CRYSTAL_BASIC:
                    attackStrategy = new BasicCrystalAttackStrategy<CrystalTrap>(transform, animator, attackPrefab, playerDetector, attackPoint);
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

    #region Animation events

    // Used on Enemy Attack animation
    public void Attack()
    {
        attackStrategy.Execute();
    }
    #endregion

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 position = attackPoint ? attackPoint.position : spawnPoint.position;
        Gizmos.DrawWireSphere(position, radius);
    }
}
