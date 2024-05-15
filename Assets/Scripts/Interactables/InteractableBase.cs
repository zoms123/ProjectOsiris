
using UnityEngine;

public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    public abstract bool Activated();
    public abstract bool CanInteract(PowerType powerType);
    public abstract void Interact();
}