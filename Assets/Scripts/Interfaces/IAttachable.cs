using UnityEngine;

public interface IAttachable
{
    public bool Attached { get; } 
    public void ChangeParent(Transform parentTransform);
}
