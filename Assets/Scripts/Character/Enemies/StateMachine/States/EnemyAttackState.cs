using UnityEngine;
using UnityEngine.AI;

public class EnemyAttackState : EnemyBaseState
{
    private NavMeshAgent agent;
    private BasicCombat basicCombat;
    private float currentTime;

    public EnemyAttackState(EnemyBase enemyBase, Animator animator, BasicCombat basicCombat, NavMeshAgent agent) : base(enemyBase, animator)
    {
        this.agent = agent;
        this.basicCombat = basicCombat;

    }

    public override void OnEnter()
    {
        agent.isStopped = true;
    }

    public override void OnExit()
    {
        agent.isStopped = false;
    }

    public override void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= basicCombat.TimeBetweenAttacks)
        {
            basicCombat.Attack();
            currentTime = 0;
        }
    }
}
