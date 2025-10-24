using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] float maxMoveSpeed;
    [SerializeField] float correctionSpeed;
    
    CharacterController controller;

    [HideInInspector] public float moveSpeed;
    [HideInInspector] public Vector3 playerStopPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //moveSpeed = MicrophoneInput.MicLoudness * maxMoveSpeed;

        if(Input.GetKey(KeyCode.W))
        {
            moveSpeed = maxMoveSpeed;
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

        controller.Move(transform.forward * moveSpeed * Time.deltaTime);

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
    }
}
