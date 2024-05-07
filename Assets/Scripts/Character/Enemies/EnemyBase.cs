using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] private PlayerDetector playerDetector;
    [SerializeField] private float attackRange;


    [Header("Patrol System")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float waitTimeOnPoint;
    [SerializeField] private float patrolSpeed;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadious;
    [SerializeField] private LayerMask groundLayerMask;

    private Vector3 currentTarget;
    private int currentIndex = -1;
    private bool playerDetected = false;
    private bool hasRecibedDamage = false;
    private GameObject player;
    private BasicCombat basicCombat;
    private StateMachine stateMachine;


    private void Awake()
    {
        stateMachine = new StateMachine();
        basicCombat = GetComponent<BasicCombat>();
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        //declare states
        var attackState = new EnemyAttackState(this, null, basicCombat, agent);
        var patrolState = new EnemyPatrolState(
            this,
            null,
            agent,
            waypoints,
            waitTimeOnPoint
            );

        var chaseState = new EnemyChaseState(this, null, agent, playerDetector);

        // declare transitions
        At(patrolState, chaseState, new FuncPredicate(() => playerDetector.CanDetectPlayer()));
        At(chaseState, attackState, new FuncPredicate(() => playerDetector.PlayerDistance(transform.position) <= attackRange));
        At(attackState, chaseState, new FuncPredicate(() => playerDetector.PlayerDistance(transform.position) > attackRange));
        
        
        Any(patrolState, new FuncPredicate(() => !playerDetector.CanDetectPlayer()));
        stateMachine.SetState(patrolState);
    }

    private void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
    private void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

    void Start()
    {
        waypoints[0].parent.SetParent(null);
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }

    private IEnumerator Patrol()
    {
        while (true)
        {
            if (IsGrounded())
            {
                  DefineNewTarget(); // Done
                  FocusTarget(currentTarget); // Done
                  while ((Vector3.Distance(transform.position, currentTarget) > 0.5))
                  {
                      if (!IsGrounded())
                      {
                          break;
                      }
                      if (!playerDetected && !hasRecibedDamage)
                      {
                          transform.position = Vector3.MoveTowards(transform.position, currentTarget, patrolSpeed * Time.deltaTime);
                          yield return null;
                      }
                      else if (hasRecibedDamage)
                      {
                          yield return new WaitForSeconds(0.5f);
                          hasRecibedDamage = false;
                      }
                      else
                      {
                          while (playerDetected)
                          {
                              if (!IsGrounded())
                              {
                                  break;
                              }
                              FocusTarget(player.transform.position);
                              while ((Vector3.Distance(transform.position, player.transform.position) > 3) && playerDetected)
                              {
                                  if (!IsGrounded())
                                  {
                                      break;
                                  }
                                  transform.position = Vector3.MoveTowards(transform.position, player.transform.position, patrolSpeed * Time.deltaTime);
                                  yield return null;
                              }
                              if (playerDetected)
                              {
                                  FocusTarget(player.transform.position);
                                  yield return new WaitForSeconds(0.5f);
                                  basicCombat.Attack();
                                  yield return new WaitForSeconds(1f);
                              }
                          }
                      }
                  }
                
                yield return new WaitForSeconds(3f);   
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    public void DefineNewTarget()
    {
        currentIndex++;
        if (currentIndex >= waypoints.Length)
        {
            currentIndex = 0;
        }
        currentTarget = waypoints[currentIndex].position;
    }

    private void FocusTarget(Vector3 target)
    {
        Vector3 relativePos = target - transform.position;
        relativePos.y = 0;
        transform.rotation = Quaternion.LookRotation(relativePos, Vector3.up);
    }

    private bool IsGrounded()
    {
        Collider[] colliders = Physics.OverlapSphere(groundCheck.transform.position, groundCheckRadious, groundLayerMask);
        return colliders.Length > 0;
    }

#region Collisions and Triggers
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("PlayerDetection"))
        {
            player = collision.gameObject;
            playerDetected = true;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("PlayerDetection"))
        {
            playerDetected = false;
        }
    }
    #endregion

    #region Debug
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(groundCheck.transform.position, groundCheckRadious);
    }
    #endregion
}
