using UnityEngine;

public class DistanceAttackStrategy : IAttackStrategy
{
    private Transform currentTarget;
    private Transform ownerTransform;
    public DistanceAttackStrategy(Transform ownerTransform)
    {
        this.ownerTransform = ownerTransform;
    }

    public void Execute()
    {
        Debug.Log("AttackingFromDistance");
        // TODO replace the instantiate for a object from a pool
        //GameObject throwableCrystal = Instantiate(throwableCrystalPrefab, firePoint.position, Quaternion.identity);
        /*GameObject throwableCrystal = null;
        Vector3 direction = ownerTransform.forward;
        if (currentTarget != null)
        {
            direction = currentTarget.position - ownerTransform.position;
        }
        throwableCrystal.GetComponent<ThrowableCrystal>().Move(direction);*/
    }

    public void SetTarget(Transform target)
    {
        currentTarget = target;
    }
}