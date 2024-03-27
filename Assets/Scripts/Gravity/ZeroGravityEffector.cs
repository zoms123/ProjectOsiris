using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ZeroGravityEffector : MonoBehaviour
{

    [SerializeField] Vector3 initialForce = new Vector3 (0, 5, 0);
    [SerializeField] float initialTime = 1;
    [SerializeField] float intervalTime = 1;
    [SerializeField] Vector3 floatingForce = new Vector3(0, 5, 0);
    [SerializeField] float floatingDragForce = 16;

    private Rigidbody rigidBody;

    private bool useZeroGravity;
    private float currentTimeInterval;

    private bool initialImpulseStoped;

    private ZeroGravityZone zeroGravityZone;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (useZeroGravity)
        {
            currentTimeInterval += Time.fixedDeltaTime;
            if (initialImpulseStoped)
            {
                GravityMotion();
            } 
            else
            {
                StopInitialImpulse();
            }            
        }
    }

    #region Methods


    private bool StopInitialImpulse()
    {        
        if (currentTimeInterval >= initialTime)
        {
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
            rigidBody.drag = floatingDragForce;
            initialImpulseStoped = true;
            currentTimeInterval = 0;
        }
        return initialImpulseStoped;
    }

    private void GravityMotion()
    {

        if (currentTimeInterval >= intervalTime)
        {
            currentTimeInterval = 0;
            rigidBody.useGravity = !rigidBody.useGravity;
            rigidBody.velocity = Vector3.zero;
        } else
        {
            rigidBody.AddForce(floatingForce);
        }
    }


    public void UseZeroGravity()
    {
        useZeroGravity = true;
        rigidBody.useGravity = false;
        rigidBody.AddForce(initialForce, ForceMode.Impulse);
    }

    public void StopUsingZeroGravity()
    {
        useZeroGravity = false;
        rigidBody.useGravity = true;
        initialImpulseStoped = false;
        rigidBody.drag = 0;
        currentTimeInterval = 0;
    }

    #endregion

    #region Collisions & triggers



    #endregion
}
