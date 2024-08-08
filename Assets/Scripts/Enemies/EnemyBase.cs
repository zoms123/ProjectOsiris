using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    [Header("Detection System")]
    [SerializeField] private PlayerDetector playerDetector;
    [SerializeField] private float attackRange;
    [SerializeField] private Transform obstaclesDetectorRayOrigin;
    [SerializeField] private LayerMask obstacleLayers;


    [Header("Patrol System")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float waitTimeOnPoint;
    [SerializeField] private float patrolSpeed;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadious;
    [SerializeField] private LayerMask groundLayerMask;

    [Header("Floating System")]
    [SerializeField] private bool canFloat = true;

    private StateMachine stateMachine;
    private NavMeshAgent agent;
    private ZeroGravityEffector zeroGravityEffector;
    private Animator animator;
    private BasicCombat basicCombat;
    private LifeSystem lifeSystem;


    private void Awake()
    {
        stateMachine = new StateMachine();
        agent = GetComponent<NavMeshAgent>();
        zeroGravityEffector = GetComponent<ZeroGravityEffector>();
        animator = GetComponentInChildren<Animator>();
        basicCombat = GetComponent<BasicCombat>();
        lifeSystem = GetComponent<LifeSystem>();

        // Declare states
        var attackState = new EnemyAttackState(this, animator, agent, playerDetector, basicCombat.AttackType);
        var patrolState = new EnemyPatrolState(
            this,
            animator,
            agent,
            waypoints,
            waitTimeOnPoint
            );

        var chaseState = new EnemyChaseState(this, animator, agent, playerDetector);

        var floatingState = new EnemyFloatingState(this, animator, agent);

        // Declare transitions
        At(patrolState, chaseState, new FuncPredicate(() => playerDetector.CanDetectPlayer()));
        At(chaseState, attackState, new FuncPredicate(() => CanAttack()));
        At(attackState, chaseState, new FuncPredicate(() => !CanAttack() || playerDetector.PlayerDistance(transform.position) > attackRange));
        if (canFloat) { At(floatingState, chaseState, new FuncPredicate(() => !zeroGravityEffector.Activated && IsGrounded())); }

        if (canFloat) { Any(floatingState, new FuncPredicate(() => zeroGravityEffector.Activated)); }
        Any(patrolState, new FuncPredicate(() => !playerDetector.CanDetectPlayer()));
        stateMachine.SetState(patrolState);
    }

    void Start()
    {
        waypoints[0].parent.SetParent(null);
    }

    void Update()
    {
        if(lifeSystem.Health > 0)
            stateMachine.Update();
    }

    #region Methods

    private void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
    private void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

    private bool CanAttack()
    {
        if (playerDetector.PlayerDistance(transform.position) <= attackRange)
        {
            Transform targetPoint = GameObject.FindGameObjectWithTag("PlayerTargetPoint").transform;
            Vector3 directionToPlayer = (targetPoint.position - obstaclesDetectorRayOrigin.position).normalized;
            float distanceToPlayer = Vector3.Distance(obstaclesDetectorRayOrigin.position, targetPoint.position);
            Debug.DrawRay(obstaclesDetectorRayOrigin.position, directionToPlayer);
            if (!Physics.Raycast(obstaclesDetectorRayOrigin.position, directionToPlayer, out RaycastHit hitInfo, distanceToPlayer, obstacleLayers, QueryTriggerInteraction.Ignore))
            {
                //Debug.Log("Raycast No hit");
                return true;
            }
            else
            {
                //Debug.Log("Raycast false" + hitInfo.collider.gameObject);
                return false;
            }
        }
        return false;
    }

    private bool IsGrounded()
    {
        Collider[] colliders = Physics.OverlapSphere(groundCheck.transform.position, groundCheckRadious, groundLayerMask);
        return colliders.Length > 0;
    }

    #endregion

    #region Debug

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.transform.position, groundCheckRadious);
    }

    #endregion
}
