using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolState : EnemyBaseState {

    private NavMeshAgent agent;
    private Vector3 startPoint;
    
    // Waipoints variables
    private Transform[] waypoints;
    private int currentIndex;
    private Vector3 currentDestination;
    private float currentTime;
    private float waitTime;

    public EnemyPatrolState(
        EnemyBase enemyBase,
        Animator animator,
        NavMeshAgent agent,
        Transform[] waypoints,
        float waitTime
        ) : base(enemyBase, animator)
    {
        this.agent = agent;
        this.waypoints = waypoints;
        this.startPoint = enemyBase.transform.position;
        this.waitTime = waitTime;
    }

    public override void OnEnter()
    {
        
    }

    public override void OnExit()
    {
        
    }

    public override void Update()
    {
        if (HasReachedDestination())
        {
            currentTime += Time.deltaTime;
            if(currentTime >= waitTime)
            {
                DefineNewDestination();
                agent.SetDestination(currentDestination);
                currentTime = 0;
            }
        }
    }

    private bool HasReachedDestination()
    {
        return !agent.pathPending
            && agent.remainingDistance <= agent.stoppingDistance
            && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
    }

    private void DefineNewDestination()
    {
        if(waypoints != null && waypoints.Length > 0)
        {
            currentIndex++;
            if (currentIndex >= waypoints.Length)
            {
                currentIndex = 0;
            }
            currentDestination = waypoints[currentIndex].position;
        }
        else
        {
            // TODO add logic to get a random point on the navMesh
        }
        
    }
}
