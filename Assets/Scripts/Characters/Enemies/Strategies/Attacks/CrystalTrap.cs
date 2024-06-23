using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalTrap : MonoBehaviour, ITrapAttack
{
    [SerializeField] private float lifetime;
    private bool initialized;
    private string ownerTag;

    public string OwnerTag { get { return ownerTag; } }

    public void Initialize(string ownerTag)
    {
        initialized = true;
        this.ownerTag = ownerTag;
        StartCoroutine(ActiveTrap());
        Invoke("ReturnToPool", lifetime);
    }

    private IEnumerator ActiveTrap()
    {
        yield return new WaitForSeconds(lifetime/2);
        gameObject.SetActive(true);
    }

    protected void ReturnToPool()
    {
        ObjectPooler.Instance.Despawn(gameObject);
        initialized = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Hit " + other.name);
            ReturnToPool();
        }
    }
}
