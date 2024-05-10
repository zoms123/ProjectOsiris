using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovable
{
    public Rigidbody GetRigidbody();

    public Vector3 GetPosition();

    public Vector3 GetLocalPosition();
}
