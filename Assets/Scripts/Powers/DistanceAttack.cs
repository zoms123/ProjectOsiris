using UnityEngine;

public abstract class DistanceAttack : MonoBehaviour, IDistanceAttack
{
    [SerializeField] protected float speed;
    [SerializeField] protected float lifetime;
    protected Vector3 direction;
    protected bool initialized;
    private GameObject ownerObject;

    public GameObject OwnerObject { get { return ownerObject; } }

    public virtual void Initialize(Vector3 direction, GameObject ownerObject)
    {
        gameObject.SetActive(true);
        initialized = true;
        this.direction = direction;
        this.ownerObject = ownerObject;
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
