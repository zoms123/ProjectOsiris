using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : CharacterManager
{
    private PlayerLocomotion locomotion;
    [HideInInspector] public PlayerAnimatorManager animatorManager;

    protected override void Awake()
    {
        base.Awake();

        locomotion = GetComponent<PlayerLocomotion>();
        animatorManager = GetComponent<PlayerAnimatorManager>();
    }

    protected override void Update()
    {
        base.Update();

        // Handle movement
        locomotion.HandleAllMovement();
    }
}
