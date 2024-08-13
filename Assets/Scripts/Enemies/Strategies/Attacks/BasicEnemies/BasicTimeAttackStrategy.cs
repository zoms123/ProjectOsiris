using UnityEngine;

public class BasicTimeAttackStrategy<T> : IAttackStrategy where T : MonoBehaviour
{
    private Transform ownerTransform;
    private GameObject attackPrefab;
    private LifeSystem ownerLifeSystem;

    public BasicTimeAttackStrategy(Transform ownerTransform, GameObject attackPrefab, LifeSystem ownerLifeSystem)
    {
        this.ownerTransform = ownerTransform;
        this.ownerLifeSystem = ownerLifeSystem;
        this.attackPrefab = attackPrefab;
        ObjectPooler.Instance.CreatePool(attackPrefab);
    }

    public void Execute()
    {
        GameObject attackObject = ObjectPooler.Instance.Spawn(attackPrefab, ownerTransform.position);
        if (attackObject != null)
        {
            T attack = attackObject.GetComponent<T>();
            if (attack != null)
            {
                (attack as IMeleeAttack).Initialize(ownerTransform.gameObject);
                ownerLifeSystem.ReceiveDamage(9999);
            }
        }
    }
}
