using UnityEngine;
using UnityEngine.AI;

public class HintFollower : MonoBehaviour
{
    [SerializeField] float lifeTime;
    float lifeTimer;
    [SerializeField] float spawnTime;
    float spawnTimer;

    [SerializeField] GameObject hintSparkPrefab;

    NavMeshAgent agent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {

        lifeTimer += Time.deltaTime;
        spawnTimer += Time.deltaTime;

        if(spawnTimer > spawnTime)
        {
            Instantiate(hintSparkPrefab, this.transform.position, Quaternion.identity);
            spawnTimer = 0;
        }

        if (lifeTimer > lifeTime)
        {
            Destroy(this.gameObject);
        }
    }
}
