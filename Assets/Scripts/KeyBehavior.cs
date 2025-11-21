using UnityEngine;

public class KeyBehavior : MonoBehaviour
{
    [SerializeField] Transform enemySpawnLocation;
    [SerializeField] Transform centerPoint;
    [SerializeField] float waypointRadius;
    [SerializeField] GameObject enemyPrefab;

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
        }
    }
}
