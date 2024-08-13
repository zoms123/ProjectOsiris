using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ThrowableRock : DistanceAttack
{
    private TrailRenderer trailRenderer;

    private void Awake()
    {
        trailRenderer = GetComponent<TrailRenderer>();
    }
    private void OnDisable()
    {
        trailRenderer.Clear();
    }

    public override void Initialize(Vector3 direction, GameObject ownerObject)
    {
        base.Initialize(direction, ownerObject);
        GetComponentInChildren<MeshRenderer>().material = ownerObject.GetComponentInChildren<SkinnedMeshRenderer>().material;
        DamageDealer damageDealer = GetComponent<DamageDealer>();
        BasicCombat basicCombat = ownerObject.GetComponent<BasicCombat>();
        damageDealer.DamageMultiplier = basicCombat.AttackDamageMultiplier;
        transform.localScale = ownerObject.transform.localScale;
        speedMultiplier = basicCombat.AttackSpeedMultiplier;
    }

    protected override void PerformAttack()
    {
         transform.Translate(direction * speed * speedMultiplier * Time.deltaTime, Space.World);
        
    }

    protected void ReturnToPool()
    {
        initialized = false;
        ObjectPooler.Instance.Despawn(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject != OwnerObject)
        {
            Debug.Log("Hit " + other.name);
            ReturnToPool();
        }
    }
}
