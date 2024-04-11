using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorManager : CharacterAnimatorManager
{
    private void OnAnimatorMove()
    {
        if (character.applyRootMotion)
        {
            Vector3 velocity = animator.deltaPosition;
            character.characterController.Move(velocity);
            transform.rotation *= animator.deltaRotation;
        }
    }
}
