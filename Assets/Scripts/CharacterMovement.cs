using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float runningSpeed;
    [SerializeField] private float lockedSpeed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float gravity = -9.81f;
    private float currentSpeed;
    private Vector3 movementDirection = Vector3.zero;
    private Vector3 velocity = Vector3.zero;
    [HideInInspector] public bool lockMovement;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckTransform;
    [SerializeField] private float detectionRadius;
    [SerializeField] private LayerMask whatIsGround;

    [Header("References")]
    [SerializeField] private InputManagerSO inputManager;
    private Transform mainCamera;
    private CharacterController controller;
    private Animator animator;

    private Vector2 inputDirection = Vector2.zero;

    private BasicCombat basicCombat;

    private void Awake()
    {
        mainCamera = Camera.main.transform;
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        basicCombat = GetComponent<BasicCombat>();
    }

    private void OnEnable()
    {
        inputManager.OnJump += Jump;
        inputManager.OnMove += Move;
        inputManager.OnRun += Run;
        inputManager.OnAttack += Attack;
    }

    private void OnDisable()
    {
        inputManager.OnJump -= Jump;
        inputManager.OnMove -= Move;
        inputManager.OnRun -= Run;
        inputManager.OnAttack -= Attack;
    }

    private void Start()
    {
        currentSpeed = movementSpeed;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Only runs when the input is being updated
    private void Move(Vector2 movementInput)
    {
        inputDirection = movementInput;
    }

    private void Jump()
    {
        if (IsGrounded())
        {
            animator.SetTrigger("Jump");
            velocity.y = Mathf.Sqrt(-2 * gravity * jumpHeight);
        }
    }

    private void Run(bool isRunPressed)
    {
        if (isRunPressed && IsGrounded())
        {
            currentSpeed = runningSpeed;
        }
        else
        {
            currentSpeed = movementSpeed;
        }
    }

    private void Attack()
    {
        if (IsGrounded())
        {
            basicCombat.Attack();
        }
    }

    private void Update()
    {
        movementDirection = mainCamera.forward * inputDirection.y + mainCamera.right * inputDirection.x;
        movementDirection.y = 0f;

        float speed = (currentSpeed != lockedSpeed) ? currentSpeed : movementSpeed;
        if (lockMovement) currentSpeed = lockedSpeed; else currentSpeed = speed;

        controller.Move(currentSpeed * Time.deltaTime * movementDirection);
        animator.SetFloat("Speed", controller.velocity.magnitude);

        animator.SetFloat("Horizontal", movementDirection.x, 0.1f, Time.deltaTime);
        animator.SetFloat("Vertical", movementDirection.y, 0.1f, Time.deltaTime);

        if (movementDirection.sqrMagnitude > 0f && !lockMovement)
        {
            RotateToTarget();
        }

        // If character has landed...
        if (IsGrounded() && velocity.y < 0)
        {
            // Then, reset its vertical velocity
            velocity.y = 0f;
            animator.ResetTrigger("Jump");
            animator.SetBool("IsFalling", false);
            animator.SetBool("IsGrounded", IsGrounded());
        }
        else
        {
            animator.SetBool("IsFalling", true);
        }
        ApplyGravity();
    }

    private void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheckTransform.position, detectionRadius, whatIsGround);
    }

    private void RotateToTarget()
    {
        Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
        transform.rotation = targetRotation;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheckTransform.position, detectionRadius);
    }
}
