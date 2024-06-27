using UnityEngine;
using UnityEngine.AI;
public class EnemyAttackState : EnemyBaseState
{
    private NavMeshAgent agent;
    private PlayerDetector playerDetector;
    private EAttackType attackType;
    private string animationAttackName;

    public EnemyAttackState(EnemyBase enemyBase, Animator animator, NavMeshAgent agent, PlayerDetector playerDetector, EAttackType attackType) : base(enemyBase, animator)
    {
        this.agent = agent;
        this.playerDetector = playerDetector;
        animationAttackName = GetAnimationAttackNameBasedOn(attackType);

    }

    private string GetAnimationAttackNameBasedOn(EAttackType attackType)
    {
        switch (attackType)
        {
            case EAttackType.GRAVITY_BASIC:
            case EAttackType.CRYSTAL_BASIC:
            case EAttackType.SHADOW_BASIC:
            case EAttackType.TIME_BASIC:
                return "BasicAttack";
            default:
                return "StrongAttack";
        }
    }

    public override void OnEnter()
    {
        animator.SetBool(animationAttackName, true);
        agent.isStopped = true;
    }

    public override void OnExit()
    {
        animator.SetBool(animationAttackName, false);
        agent.isStopped = false;
    }

    public override void Update()
    {
        Vector3 relativePos = playerDetector.Player.position - enemyBase.transform.position;
        relativePos.y = 0;
        enemyBase.transform.rotation = Quaternion.LookRotation(relativePos, Vector3.up);
    }
}
