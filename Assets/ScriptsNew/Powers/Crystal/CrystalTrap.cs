using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class CrystalTrap : DistanceAttack
{
    [SerializeField] private GameObject attackPrefab;
    private float startTime;
    private bool isMoving;
    private Vector3 startPosition;
    private float journeyLength;
    private VisualEffect visualEffect;
    private GameObject instantiatedAttack;
    private AudioSource audioSource;

    public override void Initialize(Vector3 direction, string ownerTag, Transform spawnpoint = default)
    {
        audioSource = GetComponent<AudioSource>();
        base.Initialize(direction, ownerTag);
        ObjectPooler.Instance.CreatePool(attackPrefab);
        startTime = Time.time;
        startPosition = transform.position;
        journeyLength = Vector3.Distance(startPosition, direction);
        isMoving = true;
        visualEffect = GetComponentInChildren<VisualEffect>();
        visualEffect.SetFloat("lifeTime", (journeyLength / speed));
        visualEffect.Play();
        audioSource.Play();
    }

    protected void ReturnToPool()
    {
        initialized = false;
        visualEffect.Stop();
        ObjectPooler.Instance.Despawn(instantiatedAttack);
        ObjectPooler.Instance.Despawn(gameObject);        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ReturnToPool();
        }
    }

    protected override void PerformAttack()
    {
        if (isMoving)
        {
            float distanceCovered = (Time.time - startTime) * speed;
            float fractionOfJourney = distanceCovered / journeyLength;
            transform.position = Vector3.Lerp(startPosition, direction, fractionOfJourney);
            if (fractionOfJourney >= 1.0f)
            {
                isMoving = false;
                audioSource.Stop();
                instantiatedAttack = ObjectPooler.Instance.Spawn(attackPrefab, transform.position, transform.rotation);
                instantiatedAttack.GetComponent<AudioSource>().Play();
                Invoke(nameof(ReturnToPool), lifetime);
            }
        }
    }
}
