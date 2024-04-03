using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CrystalController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputManagerSO inputManager;
    [SerializeField] private GameObject throwableCrystalPrefab;
    [SerializeField] private Transform firePoint;

    private void OnEnable()
    {
        inputManager.OnFire += Fire;
    }

    private void Fire()
    {
        Transform currentTarget = GetComponent<TargetLockOn>().CurrentTarget;
        GameObject throwableCrystal = Instantiate(throwableCrystalPrefab, firePoint.position, Quaternion.identity);
        Vector3 direction = transform.forward;
        if(currentTarget != null)
        {
            direction = currentTarget.position - transform.position;
        }
        throwableCrystal.GetComponent<ThrowableCrystal>().Move(direction);
    }
}
