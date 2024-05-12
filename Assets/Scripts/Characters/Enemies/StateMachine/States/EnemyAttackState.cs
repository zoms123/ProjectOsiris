using UnityEngine;
using UnityEngine.AI;
public class EnemyAttackState : EnemyBaseState
{
    private NavMeshAgent agent;

    public EnemyAttackState(EnemyBase enemyBase, Animator animator, NavMeshAgent agent) : base(enemyBase, animator)
    {
        this.agent = agent;

    }

    public override void OnEnter()
    {
        animator.SetBool("Attack", true);
        Debug.Log(animator.gameObject);
        agent.isStopped = true;
    }

    public override void OnExit()
    {
        animator.SetBool("Attack", false);
        agent.isStopped = false;
    }

    public override void Update()
    {
    }
}
