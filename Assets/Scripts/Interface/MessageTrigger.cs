using UnityEngine;
using System.Collections;
using TMPro;
using Unity.VisualScripting;

public class MessageTrigger : MonoBehaviour
{
    [Header("Message Attributes")]
    [SerializeField] TextMeshProUGUI messageContainer;
    [SerializeField] string message;

    [Header("Trigger Attributes")]
    [SerializeField] float messageTime;
    [SerializeField] bool destroyOnExit;

    bool hasBeenTriggered;
    float textAlpha;
    float desiredTextAlpha;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textAlpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(textAlpha != desiredTextAlpha)
        {
            //Debug.Log(textAlpha + "/" + desiredTextAlpha);

            textAlpha = Mathf.MoveTowards(textAlpha, desiredTextAlpha, 10 * Time.deltaTime);
            messageContainer.color = new Color(1, 1, 1, textAlpha);
        }
        else
        {
            if(desiredTextAlpha == 0 && hasBeenTriggered)
            {
                Destroy(this.gameObject);
            }
        }
        
    }


    private void OnTriggerEnter(Collider other)
    {
        
        if(other.CompareTag("Player"))
        {
            messageContainer.text = message;
            desiredTextAlpha = 1;

            hasBeenTriggered = true;
        }

    }

    private void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            if (destroyOnExit)
            {
                desiredTextAlpha = 0;
            }
            else
            {
                Invoke("DestroySelf", messageTime);
            }

            
        }

    }

    void DestroySelf()
    {
        desiredTextAlpha = 0;
    }

}
