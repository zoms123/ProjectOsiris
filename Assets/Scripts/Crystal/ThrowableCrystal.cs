using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableCrystal : MonoBehaviour
{
    [SerializeField] private float speed;

    private bool move;
    private Vector3 direction;

    // Update is called once per frame
    void Update()
    {
        if (move)
        {
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
        }
    }

    public void Move(Vector3 targetPosition)
    {
        move = true;
        direction = targetPosition.normalized;
        transform.rotation = Quaternion.LookRotation(direction);
    }
}
