using UnityEngine;

public class GridSpace : MonoBehaviour
{

    [SerializeField] bool isTurn;
    Vector3 playerStopPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerStopPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<PlayerMovement>())
        {
            other.gameObject.GetComponent<PlayerMovement>().playerStopPosition = this.playerStopPosition;
            if (isTurn) other.gameObject.GetComponent<PlayerMovement>().moveSpeed = 0;
        }
    }
}
