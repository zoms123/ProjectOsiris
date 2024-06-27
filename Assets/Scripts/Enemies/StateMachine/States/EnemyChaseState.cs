using UnityEngine;
using UnityEngine.AI;

public class EnemyChaseState : EnemyBaseState
{
    private readonly NavMeshAgent agent;
    private PlayerDetector playerDetector;
    private Transform target;

    public EnemyChaseState(EnemyBase enemyBase, Animator animator, NavMeshAgent agent, PlayerDetector playerDetector) : base(enemyBase, animator)
    {
        this.agent = agent;
        this.playerDetector = playerDetector;
    }

    public override void OnEnter()
    {
        animator.SetBool("Idle", false);
        animator.SetBool("BasicAttack", false);
        animator.SetBool("StrongAttack", false);
        animator.SetBool("Walk", true);
        this.target = playerDetector.Player;
    }

    public override void OnExit()
    {
        animator.SetBool("Walk", false);
    }
    public override void Update()
    {
        agent.SetDestination(target.position);
    }
}