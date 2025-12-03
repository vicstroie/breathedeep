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
    [SerializeField] float[] turnIncrements; // rotation value is weird in Unity and it doesn't really wrap to 360, it goes to 180 and -180 on the other side
    
    CharacterController controller;
    [HideInInspector] public float moveSpeed;
    [HideInInspector] public Vector3 playerStopPosition;

    [Header("External Objects")]
    [SerializeField] GameObject visionDebugger;
    [SerializeField] GameObject hintCubePrefab;
    [SerializeField] GameManager gameManager;

    int turnIndex = 0;
    float nextTurnTarget = 90; // stores the next increment of 90
    float currentRot = 0; // rotation direction works weird via script, so basically tracking an internal rotation
    float currentTurn = 0; // goes up to 90 then resets

    float stepCounter;
    bool isMoving;
    bool canMove;
    bool isSearching;
    bool turning;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        isSearching = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (PedalInput.LeftDown || PedalInput.RightDown)
        {
            if (!turning)
            {
                turning = true;
                if (transform.eulerAngles.y % 90 == 0) // only if already on a perfect increment
                {
                    nextTurnTarget = currentRot + 90 * PedalInput.InputValue;
                    currentTurn = 0;
                }
            }
            if (currentTurn + turnSpeed * Time.deltaTime >= 90) 
            {
                currentRot = nextTurnTarget;
                transform.eulerAngles = new Vector3(0, currentRot, 0);
                return; 
            }
            currentRot += turnSpeed * Time.deltaTime * PedalInput.InputValue;
            currentTurn += turnSpeed * Time.deltaTime;
            transform.eulerAngles = new Vector3(0, currentRot, 0);
        }
        else
        {
            turning = false;
        }

        //moveSpeed = MicrophoneInput.MicLoudness * maxMoveSpeed;

        if(Input.GetKeyDown(KeyCode.W))
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

        if(Input.GetKeyDown(KeyCode.U))
        {
            GameObject newCube = Instantiate(hintCubePrefab, visionDebugger.transform.position + Vector3.down, Quaternion.identity);
            newCube.GetComponent<NavMeshAgent>().SetDestination(gameManager.nextGoal.position);
        }


        if (isMoving)
        {
            if(this.transform.position != playerStopPosition)
            {
                this.transform.position = Vector3.MoveTowards(this.transform.position, playerStopPosition, correctionSpeed * Time.deltaTime);
                

            } else
            {
                isMoving = false;
                isSearching = true;
            }
        }


        #region TURNING   

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
                        if(hit.transform.gameObject.GetComponent<GridSpace>().isTurn && stepCounter != 0)
                        {
                            Debug.Log("THIS IS A TURN");
                            stepCounter = 0;
                            Debug.Log(stepCounter);
                            
                        }

                        isSearching = false;
                        isMoving = true;
                    }


                }
            } else
            {

                stepCounter = 0;

                Debug.Log("end piece");

            }
        }

        
    }

    public void MovePlayer()
    {
        stepCounter = 3;
    }

    // used by Unity Events on PedalInput
    public void Turn(float dir)
    {
        transform.Rotate(0, 90 * dir, 0);
    }
}
