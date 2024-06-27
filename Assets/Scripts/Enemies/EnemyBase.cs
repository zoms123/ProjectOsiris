using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] private PlayerDetector playerDetector;
    [SerializeField] private float attackRange;


    [Header("Patrol System")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float waitTimeOnPoint;
    [SerializeField] private float patrolSpeed;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadious;
    [SerializeField] private LayerMask groundLayerMask;

    private StateMachine stateMachine;
    private NavMeshAgent agent;
    private ZeroGravityEffector zeroGravityEffector;
    private Animator animator;
    private BasicCombat basicCombat;


    private void Awake()
    {
        stateMachine = new StateMachine();
        agent = GetComponent<NavMeshAgent>();
        zeroGravityEffector = GetComponent<ZeroGravityEffector>();
        animator = GetComponentInChildren<Animator>();
        basicCombat = GetComponent<BasicCombat>();
        //declare states
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

        // declare transitions
        At(patrolState, chaseState, new FuncPredicate(() => playerDetector.CanDetectPlayer()));
        At(chaseState, attackState, new FuncPredicate(() => playerDetector.PlayerDistance(transform.position) <= attackRange));
        At(attackState, chaseState, new FuncPredicate(() => playerDetector.PlayerDistance(transform.position) > attackRange));
        At(floatingState, chaseState, new FuncPredicate(() => !zeroGravityEffector.Activated && IsGrounded()));

        Any(floatingState, new FuncPredicate(() => zeroGravityEffector.Activated));
        Any(patrolState, new FuncPredicate(() => !playerDetector.CanDetectPlayer()));
        stateMachine.SetState(patrolState);
    }

    private void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
    private void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

    void Start()
    {
        waypoints[0].parent.SetParent(null);
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }

    private bool IsGrounded()
    {
        Collider[] colliders = Physics.OverlapSphere(groundCheck.transform.position, groundCheckRadious, groundLayerMask);
        return colliders.Length > 0;
    }

    #region Collisions and Triggers

    #endregion

    #region Debug
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(groundCheck.transform.position, groundCheckRadious);
    }
    #endregion
}
