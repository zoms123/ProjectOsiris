using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityShield : MonoBehaviour
{
    [SerializeField] private GameObject shieldPrefab;

    void Start()
    {
        Instantiate(shieldPrefab, this.gameObject.transform);
    }
}
