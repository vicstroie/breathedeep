using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemyBehavior : MonoBehaviour
{

    [Header("Attributes")]
    [SerializeField] float stoppingDistance;
    [SerializeField] float idleTime;
    float idleTimer;

    [Header("External Components")]
    [SerializeField] Transform target;
    Transform centerPoint;
    GameObject player;

    [Header("Pathfinding")]
    [SerializeField] bool usingWaypoints;
    [SerializeField] List<Transform> waypoints;
    int currentWaypoint;
    [SerializeField] float waypointRadius;


    //Components
    NavMeshAgent agent;
    EnemyFieldOfView fieldOfView;
    
    enum STATE
    {
        Idle, Patrol, Chase, Attack
    }

    STATE currentState = STATE.Idle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        fieldOfView = GetComponent<EnemyFieldOfView>();

        centerPoint = this.transform;

        SetUpNextState(STATE.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case STATE.Idle:

                //For testing
                if(Input.GetKeyDown(KeyCode.C))
                {
                    currentState = STATE.Chase;
                }

                idleTimer += Time.deltaTime;

                if (idleTimer > idleTime)
                {
                    if (usingWaypoints)
                    {
                        if (waypoints.Count > 0)
                        {
                            //Will not go to navigation if there are no waypoints and navigation relies on waypoints
                            SetUpNextState(STATE.Patrol);
                        }
                        else
                        {
                            //Stops using waypoints if no waypoints are passed in
                            usingWaypoints = false;
                        }
                    }
                    else
                    {
                        //Locates a random point in the pre-defined radius to move towards
                        //MOVED FROM PATROL - NEEDED A WAY TO FIND THE POINT BEFORE IT PATROLS, NEEDED MORE THAN ONE FRAME IN CASE POINT WAS NOT IN NAVMESH
                        Vector3 point;
                        if (FindRandomWaypoint(centerPoint.position, waypointRadius, out point)) //pass in our centre point and radius of area
                        {
                            Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f); //so you can see with gizmos
                            agent.SetDestination(point);

                            SetUpNextState(STATE.Patrol);

                        }
                    }

                }

                break;
            case STATE.Patrol:

                if (usingWaypoints)
                {
                    //Make agent walk to next patrol point
                    agent.destination = waypoints[currentWaypoint].position;

                    //Goes back into idle once it reaches its destination
                    if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
                    {

                        //Will move to next waypoint next time it enters the patrol state
                        currentWaypoint++;
                        if (currentWaypoint > waypoints.Count - 1) currentWaypoint = 0;

                        SetUpNextState(STATE.Idle);
                    }
                }
                else
                {
                    if (agent.remainingDistance <= agent.stoppingDistance) //done with path
                    {
                        //Go back to idle once waypoint is reached
                        SetUpNextState(STATE.Idle);
                    }
                }

                break;
            case STATE.Chase:

                agent.SetDestination(player.transform.position);

                if(agent.remainingDistance < agent.stoppingDistance && !agent.pathPending)
                {
                    currentState = STATE.Idle;
                }

                break;
            case STATE.Attack:
                break;
        }
    }

    //Put set ups for states here
    void SetUpNextState(STATE nextState)
    {
        switch(nextState)
        {
            case STATE.Idle:

                idleTimer = 0;
                agent.isStopped = true;

                break;
            case STATE.Patrol:

                agent.isStopped = false;

                break;
            case STATE.Chase:

                agent.isStopped = false;

                if (player == null) player = fieldOfView.playerRef;

                break;
            case STATE.Attack:
                break;
        }

        currentState = nextState;
    }

    //Finds a random point on the navmesh within a certain range, centered around a transform that the tweaker will move towards
    bool FindRandomWaypoint(Vector3 center, float range, out Vector3 result)
    {

        Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
        {
            //the 1.0f is the max distance from the random point to a point on the navmesh, might want to increase if range is big
            //or add a for loop like in the documentation
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    public void SetCenterPoint(Transform centerPoint, float range)
    {
        this.centerPoint = centerPoint;
        waypointRadius = range;
    }
}
