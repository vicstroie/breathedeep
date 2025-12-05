using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] float maxMoveSpeed;
    [SerializeField] float correctionSpeed;
    [SerializeField] float checkDistance;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float turnSpeed = 5f;
    [Tooltip("How long to wait before starting to turn again after hitting a 90 degree increment")]
    [SerializeField] float turnBufferTime = 0.5f;

    CharacterController controller;
    [HideInInspector] public float moveSpeed;
    [HideInInspector] public Vector3 playerStopPosition;

    [Header("External Objects")]
    [SerializeField] GameObject visionDebugger;
    [SerializeField] GameObject hintCubePrefab;
    [SerializeField] GameManager gameManager;

    float trueZeroRot; // the rotation at Start(), get's added to internal currentRot
    float nextTurnTarget = 90; // stores the next increment of 90
    float currentRot = 0; // rotation direction works weird via script, so basically tracking an internal rotation
    float currentTurn = 0; // goes up to 90 then resets
    float turnDir = 1; // 1 or -1, based on PedalInput.InputValue when first pressed

    float stepCounter;
    bool isMoving;
    bool canMove = true;
    bool isSearching;
    bool isTurning;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        isSearching = true;
        trueZeroRot = transform.eulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        // TODO maybe make this on leftdown/rightdown, bc if you swap keys it keeps turning (in the same direction as before)
        if (PedalInput.Left || PedalInput.Right)
        {
            if (!isTurning)
            {
                if (turnDir != PedalInput.InputValue) // if not the same as last turn input
                {
                    turnDir = PedalInput.InputValue;
                    nextTurnTarget += 90 * turnDir;
                    currentTurn = 90 - currentTurn;
                }
                
                isTurning = true;
                if (Mathf.Floor(transform.eulerAngles.y) % 90 == 0) // only if already on a perfect increment
                {
                    nextTurnTarget = currentRot + 90 * turnDir;
                    currentTurn = 0;
                }
            }

            //Debug.Log(currentTurn + turnSpeed * Time.deltaTime);
            if (currentTurn + turnSpeed * Time.deltaTime >= 90)
            {
                // reached an increment
                currentRot = nextTurnTarget;
                transform.eulerAngles = new Vector3(0, trueZeroRot + currentRot, 0);
                return;
            }
            currentRot += turnSpeed * Time.deltaTime * turnDir;
            currentTurn += turnSpeed * Time.deltaTime;
            transform.eulerAngles = new Vector3(0, trueZeroRot + currentRot, 0);
        }
        else
        {
            isTurning = false;
        }

        //moveSpeed = MicrophoneInput.MicLoudness * maxMoveSpeed;

        if (Input.GetKeyDown(KeyCode.W))
        {
            stepCounter = 3;
        }

        if (MicrophoneInput.MicLoudness > 0.5f)
        {
            moveSpeed = maxMoveSpeed;
        }
        else
        {
            if (moveSpeed > 0)
            {
                moveSpeed -= 2 * Time.deltaTime;
            }

            if (moveSpeed < 0)
            {
                moveSpeed = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            GameObject newCube = Instantiate(hintCubePrefab, visionDebugger.transform.position + Vector3.down, Quaternion.identity);
            newCube.GetComponent<NavMeshAgent>().SetDestination(gameManager.currentGoal.position);
        }


        if (isMoving)
        {
            if (this.transform.position != playerStopPosition)
            {
                this.transform.position = Vector3.MoveTowards(this.transform.position, playerStopPosition, correctionSpeed * Time.deltaTime);


            }
            else
            {
                isMoving = false;
                isSearching = true;
            }
        }


        #region isTurning   

        if (Input.GetKeyDown(KeyCode.D))
        {
            transform.Rotate(0, 90, 0);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.Rotate(0, -90, 0);
        }
        #endregion


        //controller.Move(transform.forward * moveSpeed * Time.deltaTime);

        /*
        if (moveSpeed < 2)
        {
            if (transform.position != playerStopPosition)
            {
                transform.position = Vector3.MoveTowards(transform.position, playerStopPosition, correctionSpeed * Time.deltaTime);
            } else
            {
                if (Input.GetKeyDown(KeyCode.D))
                {
                    transform.Rotate(0, 90, 0);
                }
                if (Input.GetKeyDown(KeyCode.A))
                {
                    transform.Rotate(0, -90, 0);
                }
            }



        }
        */
    }

    private void FixedUpdate()
    {

        if (isSearching)
        {
            Vector3 raycastOrigin = this.transform.position + (transform.forward * checkDistance);
            if (visionDebugger != null) { visionDebugger.transform.position = raycastOrigin; }

            RaycastHit hit;
            if (Physics.Raycast(raycastOrigin, Vector3.down, out hit))
            {

                if (hit.transform.gameObject.GetComponent<GridSpace>() != null)
                {
                    Debug.Log("found grid");

                    if (stepCounter > 0)
                    {
                        stepCounter--;
                        AudioManager.instance.PlayFootsteps();

                        playerStopPosition = hit.transform.gameObject.GetComponent<GridSpace>().playerStopPosition;
                        if (hit.transform.gameObject.GetComponent<GridSpace>().isTurn && stepCounter != 0)
                        {
                            Debug.Log("THIS IS A TURN");
                            stepCounter = 0;
                            Debug.Log(stepCounter);

                        }

                        isSearching = false;
                        isMoving = true;
                    }


                }
            }
            else
            {

                stepCounter = 0;

                //Debug.Log("end piece");

            }
        }


    }

    public void SetCanMove(bool _canMove)
    {
        canMove = _canMove;

        if (!canMove) { stepCounter = 0; }
    }

    public void MovePlayer()
    {
        if (!canMove) { return; }
        stepCounter = 4;
    }

    // used by Unity Events on PedalInput
    public void Turn(float dir)
    {
        transform.Rotate(0, 90 * dir, 0);
    }
}
