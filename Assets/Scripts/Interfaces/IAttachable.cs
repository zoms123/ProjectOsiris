using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public interface IAttachable
{
    public bool Attached { get; } 
    public void ChangeParent(Transform parentTransform);
}
