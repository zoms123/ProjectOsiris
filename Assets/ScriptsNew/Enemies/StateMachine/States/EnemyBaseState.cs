using UnityEngine;

public abstract partial class EnemyBaseState : IState
{
    protected readonly EnemyBase enemyBase;
    protected readonly Animator animator;

    public EnemyBaseState(EnemyBase enemyBase, Animator animator)
    {
        this.enemyBase = enemyBase;
        this.animator = animator;
    }

    public virtual void FixedUpdate()
    {
    }

    public virtual void OnEnter()
    {
    }

    public virtual void OnExit()
    {
    }

    public virtual void Update()
    {
    }
}