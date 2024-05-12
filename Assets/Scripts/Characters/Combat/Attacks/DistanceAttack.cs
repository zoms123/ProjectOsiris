
using UnityEngine;

public abstract class DistanceAttack : MonoBehaviour , IDistanceAttack
{
    [SerializeField] protected float speed;
    [SerializeField] private float lifetime;
    protected Vector3 direction;
    private bool initialized;
    private string ownerTag;

    public string OwnerTag { get { return ownerTag; } }

    public void Initialize(Vector3 direction, string ownerTag)
    {
        this.direction = direction;
        initialized = true;
        this.ownerTag = ownerTag;
        Invoke("ReturnToPool", lifetime);
    }

    protected void Update()
    {
        if (initialized)
        {
            PerformAction();
        }
    }

    protected abstract void PerformAction();

    protected void ReturnToPool()
    {
        ObjectPool.ReturnObject(gameObject);
        initialized = false;
    }
}