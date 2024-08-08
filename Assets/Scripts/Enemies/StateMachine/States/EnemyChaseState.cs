using System.IO;
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
        agent.isStopped = false;
        animator.SetBool("Idle", false);
        animator.SetBool("BasicAttack", false);
        animator.SetBool("StrongAttack", false);
        animator.SetBool("Walk", true);
        this.target = playerDetector.Player;
    }

    public override void OnExit()
    {
        
        agent.ResetPath();
        agent.isStopped = true;
        animator.SetBool("Walk", false);
    }
    public override void Update()
    {
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(target.position, path);
        if (path.status == NavMeshPathStatus.PathComplete)
        {
            agent.SetDestination(target.position);
        } else
        {
            agent.ResetPath();
        }
    }
}