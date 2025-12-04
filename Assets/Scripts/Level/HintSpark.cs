using UnityEngine;

public class HintSpark : MonoBehaviour
{

    [SerializeField] float lifeTime;
    float lifeTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        lifeTimer += Time.deltaTime;

        if (lifeTimer > lifeTime)
        { 
            Destroy(this.gameObject);
        }
    }
}
