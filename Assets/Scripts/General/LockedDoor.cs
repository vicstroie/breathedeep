using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    public bool isUnlocked;

    [SerializeField] GameManager manager;
    [SerializeField] GameObject key;

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
        if (isUnlocked)
        {
            Debug.Log("You survived!");
            SceneLoader.instance.FadeOutAndLoad(3);
        } else
        {
            Debug.Log("Need KEY");

            manager.SetCurrentGoal(key.transform);

        }
    }


}
