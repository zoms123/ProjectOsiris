using UnityEngine;

public class SphereDetectionStrategy : IDetectionStrategy
{
    private Transform owner;
    private LayerMask layer;
    private string tag;
    private float sphereRadius;
    private Transform target;

    public SphereDetectionStrategy(Transform owner, float sphereRadius, LayerMask layer, string tag)
    {
        this.owner = owner;
        this.sphereRadius = sphereRadius;
        this.layer = layer;
        this.tag = tag;
    }
    public bool Execute(out Transform foundTarget)
    {
        if (!target)
        {
            Collider[] colliders = Physics.OverlapSphere(owner.position, sphereRadius, layer);
            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag(tag))
                {
                    foundTarget = target = collider.transform;
                    return true;
                }
            }
        } 
        else
        {
            if(Vector3.Distance(owner.position, target.position) <= sphereRadius)
            {
                foundTarget = target;
                return true;
            }
        }
        foundTarget = null;
        return false;
    }
}