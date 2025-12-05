using TMPro;
using UnityEngine;

public class EnemySpawnTrigger : MonoBehaviour
{

    [Header("Enemy Spawn")]
    [SerializeField] Transform enemySpawnLocation;
    [SerializeField] Transform firstWaypoint;

    [Header("Prefabs")]
    [SerializeField] GameObject enemyObject;

    [Header("Message Attributes")]
    [SerializeField] TextMeshProUGUI messageContainer;
    [SerializeField] string message;

    [Header("Trigger Attributes")]
    [SerializeField] float messageTime;
    float messageTimer;
    [SerializeField] bool destroyOnExit;

    bool hasBeenTriggered;
    float textAlpha;
    float desiredTextAlpha;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if(hasBeenTriggered)
        {
            messageTimer += Time.deltaTime;

            if (messageTimer > messageTime)
            {
                desiredTextAlpha = 0;
            }
        }

        if (textAlpha != desiredTextAlpha)
        {
            Debug.Log(textAlpha + "/" + desiredTextAlpha);

            textAlpha = Mathf.MoveTowards(textAlpha, desiredTextAlpha, 10 * Time.deltaTime);
            messageContainer.color = new Color(1, 1, 1, textAlpha);
        }
        else
        {
            if (desiredTextAlpha == 0 && hasBeenTriggered)
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            enemyObject.SetActive(true);
            enemyObject.GetComponent<EnemyBehavior>().SetFirstPoint(firstWaypoint);


            messageContainer.text = message;
            desiredTextAlpha = 1;

            hasBeenTriggered = true;
        }
    }
}
