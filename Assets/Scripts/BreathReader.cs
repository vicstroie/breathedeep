using NUnit.Framework;
using System.Collections;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.IO.Ports;

public class BreathReader : MonoBehaviour
{
    bool breathe_in = false; // false = breathe out
    float sensorADC = 0; // the value returned by the analog port the stretch sensor is plugged into

    float adcMin = 10000;
    float adcMax = 0;

    [Header("Arduino")]
    [SerializeField] public string portName = "COM3";
    [SerializeField] public int baudRate = 9600;

    [Space(10)]
    [SerializeField] KeyCode startSampleKey = KeyCode.Space;
    bool sampling = true;
    bool calibrating = false;

    [SerializeField] UnityEvent onBreatheIn;
    [SerializeField] UnityEvent onBreatheOut;

    [SerializeField] int sampleFrames = 15;

    Queue<float> adcAverage = new Queue<float>();
    float lastSample;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //sensorADC = FindFirstObjectByType<SerialReader>().GetValue();
        //Debug.Log(FindFirstObjectByType<SerialController>().ReadSerialMessage());


        if (Input.GetKeyDown(startSampleKey))
        {
            calibrating = !calibrating;
        }

        if (calibrating)
        {
            if (sensorADC > adcMax) { adcMax = sensorADC; }
            if (sensorADC <adcMin) { adcMin = sensorADC; }
        }

        if (sampling)
        {
            if (adcAverage.Count >= sampleFrames) { adcAverage.Dequeue(); }
            adcAverage.Enqueue(sensorADC);

            float total = 0;
            foreach (float val in adcAverage) { total += val; }
            float currentSample = total / adcAverage.Count;

            lastSample = currentSample;
        }

        
    }

    public void OnMessageArrived(string msg)
    {
        sensorADC = float.Parse(msg);
    }

    void OnConnectionEvent(bool success)
    {
        if (success)
            Debug.Log("Connection established");
        else
            Debug.Log("Connection attempt failed or disconnection detected");
    }

    IEnumerator Calibrate()
    {
        yield return null;
    }
}
