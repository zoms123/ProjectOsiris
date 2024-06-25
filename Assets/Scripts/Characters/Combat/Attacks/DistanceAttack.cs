
using UnityEngine;

public abstract class DistanceAttack : MonoBehaviour , IDistanceAttack
{
    [SerializeField] protected float speed;
    [SerializeField] protected float lifetime;
    protected Vector3 direction;
    protected bool initialized;
    private string ownerTag;

    public string OwnerTag { get { return ownerTag; } }

    public virtual void Initialize(Vector3 direction, string ownerTag, Transform spawnpoint = default)
    {
        gameObject.SetActive(true);
        initialized = true;
        this.direction = direction;
        this.ownerTag = ownerTag;
    }

    protected void Update()
    {
        if (initialized)
        {
            PerformAttack();
        }
    }

    protected abstract void PerformAttack();
}