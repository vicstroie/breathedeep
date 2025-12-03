using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] Material keyMaterial;
    [SerializeField] Color keyColor;

    public Transform nextGoal;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        Debug.Log(keyMaterial.color.a);

        if(Input.GetKey(KeyCode.W))
        {
            if (keyMaterial.color != new Color(keyColor.r, keyColor.g, keyColor.b, 1))
            {
                keyMaterial.color = Color.Lerp(keyMaterial.color, new Color(keyColor.r, keyColor.g, keyColor.b, 1), 10 * Time.deltaTime);
            }
        }
        else
        {
            if(keyMaterial.color != new Color(keyColor.r, keyColor.g, keyColor.b, 0))
            {
                keyMaterial.color = Color.Lerp(keyMaterial.color, new Color(keyColor.r, keyColor.g, keyColor.b, 0), 10 * Time.deltaTime);
            }
        }


            
    }
}
