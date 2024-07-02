using UnityEngine;
using UnityEngine.AI;

public class EnemyFloatingState : EnemyBaseState
{
    private NavMeshAgent agent;

    public EnemyFloatingState(EnemyBase enemyBase, Animator animator, NavMeshAgent agent) : base(enemyBase, animator)
    {
        this.agent = agent;
    }

    public override void OnEnter()
    {
        Debug.Log("Entering Floating State");
        animator.SetTrigger("Floating");
        agent.isStopped = true;
        agent.enabled = false;
    }

    public override void OnExit()
    {
        Debug.Log("Exit Floating State");
        animator.ResetTrigger("Floating");
        agent.enabled = true;
        agent.isStopped = false;
    }
}
