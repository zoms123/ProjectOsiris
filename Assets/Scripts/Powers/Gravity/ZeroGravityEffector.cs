using UnityEngine;

public class ZeroGravityEffector : MonoBehaviour, IZeroGravityEffector
{

    [SerializeField] bool applyInitialForce;
    [SerializeField] Vector3 initialForce = new Vector3 (0, 5, 0);
    [SerializeField] float initialTime = 1;
    [SerializeField] float intervalTime = 1;
    [SerializeField] Vector3 floatingForce = new Vector3(0, 5, 0);
    [SerializeField] float floatingDragForce = 16;

    private Rigidbody rigidBody;

    private bool useZeroGravity;
    private float currentTimeInterval;

    private bool initialImpulseStoped;

    public bool Activated { get { return useZeroGravity; } }

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
            rigidBody.velocity = Vector3.zero;
            rigidBody.AddForce(floatingForce);
        }
    }


    public void UseZeroGravity()
    {
        useZeroGravity = true;
        rigidBody.useGravity = false;
        rigidBody.velocity = Vector3.zero;
        if(applyInitialForce)
            rigidBody.AddForce(initialForce, ForceMode.Impulse);
    }

    public void StopUsingZeroGravity()
    {
        useZeroGravity = false;
        if (rigidBody != null) 
        {
            rigidBody.velocity = Vector3.zero;
            rigidBody.useGravity = true;
            rigidBody.drag = 0;
        }
        initialImpulseStoped = false;
        currentTimeInterval = 0;
    }

    #endregion

 }
