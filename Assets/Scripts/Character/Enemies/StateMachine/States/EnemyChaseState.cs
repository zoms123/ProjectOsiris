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
        // set animation properties
        this.target = playerDetector.Player;
    }

    public override void OnExit()
    {
    }
    public override void Update()
    {
        agent.SetDestination(target.position);
    }
}