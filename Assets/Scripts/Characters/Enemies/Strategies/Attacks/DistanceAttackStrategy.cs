using UnityEngine;

public class DistanceAttackStrategy<T> : IAttackStrategy where T : MonoBehaviour
{
    private Transform currentTarget;
    private Transform ownerTransform;
    private Animator animator;
    private GameObject attackPrefab;
    private Transform attackPoint;

    public DistanceAttackStrategy(Transform ownerTransform, Animator animator, GameObject attackPrefab, Transform attackPoint)
    {
        this.animator = animator;
        this.ownerTransform = ownerTransform;
        this.attackPrefab = attackPrefab;
        this.attackPoint = attackPoint;
        ObjectPool.Initialize(attackPrefab);
    }

    public void Execute()
    {
        GameObject attackObject = ObjectPool.GetObject(attackPrefab);
        if (attackObject != null)
        {
            attackObject.transform.position = attackPoint.position;
            attackObject.transform.rotation = attackPoint.rotation;
            T attack = attackObject.GetComponent<T>();
            if (attack != null)
            {
                Vector3 direction = attackPoint.forward;
                if (currentTarget != null)
                {
                    direction = (currentTarget.position - attackPoint.position).normalized;                    
                }
                (attack as IDistanceAttack).Initialize(direction);
            }
        }
    }

    public void SetTarget(Transform target)
    {
        currentTarget = target;
    }
}