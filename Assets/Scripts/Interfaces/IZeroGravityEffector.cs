using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IZeroGravityEffector
{
    public bool Activated { get; }

    public void UseZeroGravity();

    public void StopUsingZeroGravity();
}
