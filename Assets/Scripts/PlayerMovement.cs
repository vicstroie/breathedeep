using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float maxMoveSpeed;
    float moveSpeed;
    CharacterController controller;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W))
        {
            moveSpeed = maxMoveSpeed;
        }

        if (moveSpeed > 0)
        { 
            moveSpeed -= 2 * Time.deltaTime;
        }

        if (moveSpeed < 0)
        {
            moveSpeed = 0;
        }

        controller.Move(transform.forward * moveSpeed * Time.deltaTime);

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
