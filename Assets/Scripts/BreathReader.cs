using System.Security.Policy;
using UnityEngine;
using UnityEngine.Events;

public class BreathReader : MonoBehaviour
{
    bool breathe_in = false; // false = breathe out
    float sensorADC = 0; // the value returned by the analog port the stretch sensor is plugged into

    float adcMin = 0;
    float adcMax = 0;

    [SerializeField] UnityEvent onBreatheIn;
    [SerializeField] UnityEvent onBreatheOut;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
