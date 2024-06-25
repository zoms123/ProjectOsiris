using UnityEngine;

public class BasicGravityAttackStrategy<T> : IAttackStrategy where T : MonoBehaviour
{
    private Transform currentTarget;
    private Transform ownerTransform;
    private Animator animator;
    private GameObject attackPrefab;
    private Transform attackPoint;

    public BasicGravityAttackStrategy(Transform ownerTransform, Animator animator, GameObject attackPrefab, Transform attackPoint)
    {
        this.animator = animator;
        this.ownerTransform = ownerTransform;
        this.attackPrefab = attackPrefab;
        this.attackPoint = attackPoint;
        ObjectPooler.Instance.CreatePool(attackPrefab);
    }

    public void Execute()
    {
        Debug.Log("prefab " + attackPrefab);
        GameObject attackObject = ObjectPooler.Instance.Spawn(attackPrefab, attackPoint.position, attackPoint.rotation);
        Debug.Log("attackObject " + attackObject);
        if (attackObject != null)
        {
            T attack = attackObject.GetComponent<T>();
            if (attack != null)
            {
                Vector3 direction = attackPoint.forward;
                if (currentTarget != null)
                {
                    direction = (currentTarget.position - attackPoint.position).normalized;
                }
                (attack as IDistanceAttack).Initialize(direction, ownerTransform.tag);
            }
        }
    }

    public void SetTarget(Transform target)
    {
        currentTarget = target;
    }
}