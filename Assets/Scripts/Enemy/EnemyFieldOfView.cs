using System.Collections;
using System.Linq.Expressions;
using UnityEngine;

public class EnemyFieldOfView : MonoBehaviour
{
    public float radius;
    [Range(0, 360)] public float angle;

    public GameObject playerRef;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    public bool canSeePlayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        //targetMask will only include the player
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        //If targetMask included more gameobjects than the player, you would have to cycle through the array
        if (rangeChecks.Length > 0) {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            //Checks if player is within eyesight
            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                //Player within eyesight
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                //Check if player is behind a wall
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    //Player is VISIBLE
                    canSeePlayer = true;
                }
                else
                {
                    canSeePlayer = false;
                }

            } else
            {
                //Player not within eyesight
                canSeePlayer = false;
            }
        } else if(canSeePlayer)
        {
            //turns off canSeePlayer in case player moved out of sightline
            canSeePlayer = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
