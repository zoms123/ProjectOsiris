using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public bool CanInteract(PowerType powerType);
    
    public void Interact();

    public bool Activated();
}
