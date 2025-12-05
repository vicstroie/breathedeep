using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

    [SerializeField] Material keyMaterial;
    [SerializeField] Color keyColor;

    public Transform currentGoal;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


            
    }

    public void SetCurrentGoal(Transform goal)
    {
        currentGoal = goal;
    }
}
