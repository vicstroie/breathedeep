using UnityEngine;

public class KeyBehavior : MonoBehaviour
{
    [Header("Enemy Spawn")]
    [SerializeField] Transform enemySpawnLocation;
    [SerializeField] Transform centerPoint;
    [SerializeField] float waypointRadius;

    [Header("Prefabs")]
    [SerializeField] GameObject enemyPrefab;

    [Header("Other")]
    [SerializeField] LockedDoor lockedDoor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            GameObject newEnemy = Instantiate(enemyPrefab, enemySpawnLocation.position, Quaternion.identity);
            newEnemy.GetComponent<EnemyBehavior>().SetCenterPoint(centerPoint, waypointRadius);

            lockedDoor.isUnlocked = true;

            Destroy(this.gameObject);
        }
    }
}
