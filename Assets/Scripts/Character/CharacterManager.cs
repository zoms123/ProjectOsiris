using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class CharacterManager : MonoBehaviour
{
    public CharacterController characterController;

    [Header("Flags")]
    public bool isPerformingAction = false;
    public bool isGrounded = true;
    public bool isSprinting = false;
    public bool isJumping = false;
    public bool isStrafing = false;
    public bool applyRootMotion = false;
    public bool canRotate = true;
    public bool canMove = true;

    protected virtual void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }
}
