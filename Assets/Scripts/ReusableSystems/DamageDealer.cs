using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField] private float damage;
    private float damageMultiplier = 1;

    public float DamageMultiplier { get { return damageMultiplier; } set { damageMultiplier = value; } }
    public float Damage { get { return damage * damageMultiplier; } set { damage = value; } }

}
