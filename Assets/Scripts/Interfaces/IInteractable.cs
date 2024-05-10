using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public event Action OnLoseObject;

    public bool CanInteract(PowerType powerType);
    
    public void Interact();

    public bool Activated();
}
