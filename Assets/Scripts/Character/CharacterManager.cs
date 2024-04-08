using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public Animator animator;

    [Header("Flags")]
    public bool isPerformingAction = false;
    public bool isGrounded = true;
    public bool isSprinting = false;
    public bool isJumping = false;
    public bool isStrafing = false;
    public bool applyRootMotion = false;
    public bool canRotate = true;
    public bool canMove = true;

    private int grounded;

    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);

        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        grounded = Animator.StringToHash("IsGrounded");
    }

    protected virtual void Update()
    {
        animator.SetBool(grounded, isGrounded);
    }
}
