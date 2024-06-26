using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovable
{
    public Rigidbody GetRigidbody();

    public Vector3 GetPosition();

    public Vector3 GetLocalPosition();

    public void SetPosition(Vector3 position);

    public void SetLocalPosition(Vector3 localPosition);
}
