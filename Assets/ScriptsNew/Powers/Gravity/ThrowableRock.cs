using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ThrowableRock : DistanceAttack
{
    [SerializeField] private GameObject attackPrefab;
    private Transform spawnpoint;
    private VisualEffect visualEffect;
    private GameObject instantiatedAttack;
    private bool instantiated;

    public override void Initialize(Vector3 direction, string ownerTag, Transform spawnpoint)
    {
        base.Initialize(direction, ownerTag);
        this.spawnpoint = spawnpoint;
        visualEffect = GetComponentInChildren<VisualEffect>();
        visualEffect.Play();
        ObjectPooler.Instance.CreatePool(attackPrefab);
    }
    protected override void PerformAttack()
    {
        float distance = Vector3.Distance(transform.position, spawnpoint.position);
        if (distance < 0.01 && !instantiated)
        {
            Vector3 relativePos = direction - transform.position;
            relativePos.y = 0;
            instantiatedAttack = ObjectPooler.Instance.Spawn(attackPrefab, spawnpoint.position, Quaternion.LookRotation(relativePos * -1, Vector3.up));
            instantiated = true;
            visualEffect.Stop();
        } else if (instantiated)
        {
            instantiatedAttack.transform.Translate(speed * Time.deltaTime * direction, Space.World);
        }
    }

    protected void ReturnToPool()
    {
        initialized = false;
        instantiated = false;
        ObjectPooler.Instance.Despawn(instantiatedAttack);
        ObjectPooler.Instance.Despawn(gameObject);
    }

    // ESTA FUNCION NO SE ESTA EJECUTANDO NUNCA PORQUE EL PREFAB "GravityThrowableAttack" NO TIENE COLLIDER
    // Y POR TANTO, "attackPrefab" QUE EN ESTE CASO ES "ThrowableRock", NO SE DESTRUYE AL GOLPEAR OTRO OBJETO
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(OwnerTag))
        {
            Debug.Log("Hit " + other.name);
            ReturnToPool();
        }
    }
}
