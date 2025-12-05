using UnityEngine;

public class KeyBehavior : MonoBehaviour
{
    [Header("Other")]
    [SerializeField] LockedDoor lockedDoor;
    [SerializeField] GameManager gameManager;
    [SerializeField] EnemySpawnTrigger trigger;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trigger.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            lockedDoor.isUnlocked = true;
            gameManager.SetCurrentGoal(lockedDoor.transform);
            trigger.gameObject.SetActive(true);

            AudioManager.instance.PlayKeyPickup();
            Destroy(this.gameObject);
        }
    }
}
