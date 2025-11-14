using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemyBehavior : MonoBehaviour
{
    [Header("External Components")]
    [SerializeField] Transform target;

    [Header("Pathfinding")]
    [SerializeField] bool usingWaypoints;
    [SerializeField] List<Transform> waypoints;
    int currentWaypoint;


    //Components
    NavMeshAgent agent;

    

    enum STATE
    {
        Idle, Patrol, Chase, Attack
    }

    STATE currentState = STATE.Idle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case STATE.Idle:

                if(Input.GetKeyDown(KeyCode.C))
                {
                    currentState = STATE.Chase;
                }

                break;
            case STATE.Patrol:
                break;
            case STATE.Chase:

                agent.SetDestination(target.position);

                if(agent.remainingDistance < agent.stoppingDistance && !agent.pathPending)
                {
                    currentState = STATE.Idle;
                }

                break;
            case STATE.Attack:
                break;
        }
    }
}
