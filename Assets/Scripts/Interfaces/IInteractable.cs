using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public bool CanInteract();
    
    public void Interact(PowerType powerType);
}
