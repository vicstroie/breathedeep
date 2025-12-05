using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public class EnemyBehavior : MonoBehaviour
{

    [Header("Attributes")]
    [SerializeField] float stoppingDistance;
    [SerializeField] float idleTime;
    float idleTimer;
    [SerializeField] float roamSpeed;
    [SerializeField] float chaseSpeed;
    [SerializeField] float stalkSpeed;

    [Header("External Components")]
    [SerializeField] Transform target;
    Transform firstWaypoint;
    GameObject player;

    [Header("Pathfinding")]
    [SerializeField] bool usingWaypoints;
    [SerializeField] List<Transform> waypoints;
    int currentWaypoint;
    [SerializeField] float waypointRadius;

    [Header("Visual Feedback")]
    [SerializeField] float minimumFogDistance;

    [Header("Attack State")]
    [SerializeField] float attackRange = 10f;
    [SerializeField] float attackMoveSpeed = 1f;
    [SerializeField] float attackCamFOV = 90;
    [SerializeField] float breathHoldTime = 4.5f;
    [SerializeField] GameObject holdWarningText;
    float breathTimer = 0;
    float defaultCamFOV;
    bool easingFOV = false;

    //Components
    NavMeshAgent agent;
    EnemyFieldOfView fieldOfView;
    
    enum STATE
    {
        SetUp, Idle, Patrol, Chase, Attack
    }

    STATE currentState = STATE.SetUp;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        holdWarningText.SetActive(false);

        agent = GetComponent<NavMeshAgent>();
        fieldOfView = GetComponent<EnemyFieldOfView>();
        player = GameObject.FindGameObjectWithTag("Player");
        defaultCamFOV = Camera.main.fieldOfView;

        SetUpNextState(STATE.SetUp);

        AudioManager.instance.PlayMonsterSpawn();
        AudioManager.instance.PlayMonsterAmbience();
        AudioManager.instance.SetMonsterTransform(transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) { SetUpNextState(STATE.Attack); } // REMOVE THIS AFTER SETTING IT UP TO MONSTER STATES


        float playerDistance = 0;

        if (player != null)
        {
            playerDistance = Vector3.Distance(this.transform.position, player.transform.position);
        } else
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

            /*
            float fogDelta = 0;


            //Debug.Log(playerDistance);

            if (playerDistance < minimumFogDistance)
            { 
                fogDelta = (1 / playerDistance) + 0.4f;
            } else
            {
                fogDelta = 0.2f;
            }


            if (RenderSettings.fogDensity != fogDelta) RenderSettings.fogDensity = Mathf.MoveTowards(RenderSettings.fogDensity, fogDelta, 2 * Time.deltaTime);
            */

            switch (currentState)
            {

                case STATE.SetUp:


                    if (firstWaypoint != null)
                    {
                        agent.SetDestination(firstWaypoint.position);
                        agent.isStopped = false;
                        agent.speed = roamSpeed;
                    }

                    if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
                    {
                        SetUpNextState(STATE.Patrol);
                    }



                    break;

                case STATE.Idle:

                    //For testing
                    if (Input.GetKeyDown(KeyCode.C))
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
                            if (FindRandomWaypoint(player.transform.position, waypointRadius, out point)) //pass in our centre point and radius of area
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
                        agent.SetDestination(waypoints[currentWaypoint].position);

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

                    if (playerDistance < attackRange)
                    {
                        SetUpNextState(STATE.Attack);
                    }

                    break;
                case STATE.Chase:

                    agent.SetDestination(player.transform.position);

                    if (playerDistance < attackRange)
                    {
                        SetUpNextState(STATE.Attack);
                    }

                    break;
                case STATE.Attack:
                    agent.isStopped = true;

                    float fovRate = (attackCamFOV - defaultCamFOV) / breathHoldTime - 0.5f;
                    float abbRate = 1 / breathHoldTime;
                    float vignetteRate = 0.25f / breathHoldTime;

                    // zoom in and ease chromatic aberration as holding breath
                    if (!easingFOV)
                    {
                        Camera.main.fieldOfView -= fovRate * Time.deltaTime;
                        PostProcessControl.instance.aberrationIntesity -= abbRate * Time.deltaTime;
                        PostProcessControl.instance.vignetteIntensity -= vignetteRate * Time.deltaTime;
                    }

                    // count up
                    if (breathTimer < breathHoldTime)
                    {
                        breathTimer += Time.deltaTime;
                    }
                    else
                    {
                        // resume normal movement and player state
                        StartCoroutine(EaseFOV(defaultCamFOV, -5.5f));
                        PostProcessControl.instance.aberrationIntesity = 0;
                        PostProcessControl.instance.vignetteIntensity = PostProcessControl.instance.defaultVignette;
                        holdWarningText.SetActive(false);
                        FindFirstObjectByType<PlayerMovement>().SetCanMove(true);

                        // FOR VICTOR
                        // monster should walk away/retreat/etc
                        // maybe go back to a waypoint and start patrolling again?
                        Vector3 point;
                        if (FindRandomWaypoint(player.transform.position, waypointRadius, out point)) //pass in our centre point and radius of area
                        {
                            Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f); //so you can see with gizmos
                            agent.SetDestination(point);

                            SetUpNextState(STATE.Patrol);

                        }
                    }

                    // if close enough, kill
                    if (agent.remainingDistance < agent.stoppingDistance && !agent.pathPending)
                    {
                        Debug.Log("You lost");
                        SceneManager.LoadScene("Death");
                    }

                    
                    break;
            }

        //AudioManager.instance.ChangeMonsterDistanceParameter(playerDistance);
        //AudioManager.instance.ChangeMonsterDirectionParameter(Vector3.Angle(player.transform.position, transform.position));
    }

    // called by BreathReader OnStateChange unity event
    public void BreakBreathHold()
    {
        if (currentState == STATE.Attack)
        {
            StartAttack();

            agent.isStopped = false;
            agent.speed = stalkSpeed;


            // FOR VICTOR
            // monster should inch closer, not sure what best way to do that is, i never use navmesh lol
        }
    }

    IEnumerator EaseFOV(float target, float displacement)
    {
        easingFOV = true;
        float fov = Camera.main.fieldOfView;
        if (Mathf.Sign(displacement) == -1) // zoom in
        {
            while (fov > target)
            {
                if (fov + displacement * Time.deltaTime > target)
                {
                    fov += displacement * Time.deltaTime;
                    Camera.main.fieldOfView = fov;
                    yield return null;
                }
                else
                {
                    fov = target;
                    Camera.main.fieldOfView = fov;
                }
            }
            Camera.main.fieldOfView = fov;
            easingFOV = false;
        }
        else // zoom out
        {
            while (fov < target)
            {
                if (fov + displacement * Time.deltaTime < target)
                {
                    fov += displacement * Time.deltaTime;
                    Camera.main.fieldOfView = fov;
                    yield return null;
                }
                else
                {
                    fov = target;
                    Camera.main.fieldOfView = fov;
                }
            }
            Camera.main.fieldOfView = fov;
            easingFOV = false;
        }
    }

    void StartAttack()
    {
        breathTimer = 0;
        CameraControl.instance.ScreenShake(0.3f, 0.15f);
        // set cam fov high
        StartCoroutine(EaseFOV(attackCamFOV, 145));
        // turn up chromatic aberration and make vignette tighter
        PostProcessControl.instance.aberrationIntesity = 1;
        PostProcessControl.instance.vignetteIntensity = 0.45f;
        // warn to hold breath
        holdWarningText.SetActive(true);

        AudioManager.instance.PlayPlayerHit();
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

                agent.speed = roamSpeed;

                break;
            case STATE.Chase:

                agent.isStopped = false;

                agent.speed = chaseSpeed;

                if (player == null) player = fieldOfView.playerRef;

                break;
            case STATE.Attack:
                StartAttack();
                FindFirstObjectByType<PlayerMovement>().SetCanMove(false);
                break;
        }

        currentState = nextState;
    }

    //Finds a random point on the navmesh within a certain range, centered around a transform that the tweaker will move towards
    bool FindRandomWaypoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere 
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 8f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
            {
                //the 1.0f is the max distance from the random point to a point on the navmesh, might want to increase if range is big
                //or add a for loop like in the documentation
                result = hit.position;
                return true;
            }
        }

        result = Vector3.zero;
        return false;
    }

    public void SetFirstPoint(Transform centerPoint)
    {
        firstWaypoint = centerPoint;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player is DEAD");
        }
    }
}
