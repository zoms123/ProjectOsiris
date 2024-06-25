using UnityEngine;
using UnityEngine.AI;
public class EnemyAttackState : EnemyBaseState
{
    private NavMeshAgent agent;
    private PlayerDetector playerDetector;

    public EnemyAttackState(EnemyBase enemyBase, Animator animator, NavMeshAgent agent, PlayerDetector playerDetector) : base(enemyBase, animator)
    {
        this.agent = agent;
        this.playerDetector = playerDetector;
    }

    public override void OnEnter()
    {
        animator.SetBool("Attack", true);
        agent.isStopped = true;
    }

    public override void OnExit()
    {
        animator.SetBool("Attack", false);
        agent.isStopped = false;
    }

    public override void Update()
    {
        Vector3 relativePos = playerDetector.Player.position - enemyBase.transform.position;
        relativePos.y = 0;
        enemyBase.transform.rotation = Quaternion.LookRotation(relativePos, Vector3.up);
    }
}
