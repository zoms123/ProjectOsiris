using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestoreLifeSystem : MonoBehaviour
{
    [Header("Lifes System")]
    [SerializeField] private float waitRestoreTime;
    [SerializeField] private float lifeRestoredPerSecond;

    private LifeSystem lifeSystem;

    private float lastRestore = 0f;

    private void Awake()
    {
        lifeSystem = GetComponent<LifeSystem>();
    }

    private void Update()
    {
        if (lifeSystem && lifeSystem.Health < lifeSystem.MaxHealth && lifeSystem.TimeLastAttack > waitRestoreTime && lastRestore >= 1f)
        {
            lifeSystem.RestoreLife(lifeRestoredPerSecond);
            lastRestore = 0f;
        }
        lastRestore += Time.deltaTime;
    }
}
