using NUnit.Framework;
using System.Collections;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using System.IO.Ports;
using TMPro;

public class BreathReader : MonoBehaviour
{
    bool breatheIn = false; // false = breathe out
    float sensorADC = 0; // the value returned by the analog port the stretch sensor is plugged into
    float lastReading;

    float adcMin = 10000;
    float adcMax = 0;

    [SerializeField] bool debug = false;
    [SerializeField] TMP_Text adcText;
    [SerializeField] TMP_Text avgText;
    [SerializeField] TMP_Text minMaxText;
    [SerializeField] GameObject calibratingText;
    [SerializeField] Image bg;
    Color red;

    [Space(10)]
    [SerializeField] KeyCode startSampleKey = KeyCode.Space;
    bool sampling = true;
    bool calibrating = false;
    bool calibrated = false;

    [SerializeField] UnityEvent onBreatheIn;
    [SerializeField] UnityEvent onBreatheOut;

    [SerializeField] int sampleFrames = 15;
    [SerializeField] float sensThreshold = 5f; // should calibrate at start by having them stand still/hold their breath

    Queue<float> adcAverage = new Queue<float>(); // holds the values from the last 60 frames (or whatever the value of sampleFrames is) to average it
    Queue<float> samples = new Queue<float>();
    float currentSample;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (debug) { red = bg.color; }
    }

    // Update is called once per frame
    void Update()
    {
        //sensorADC = FindFirstObjectByType<SerialReader>().GetValue();
        //Debug.Log(FindFirstObjectByType<SerialController>().ReadSerialMessage());


        if (Input.GetKeyDown(startSampleKey))
        {
            calibrating = !calibrating;

            if (debug) { calibratingText.SetActive(calibrating); }

            if (!calibrating)
            {
                if (debug) { minMaxText.text = "Min, Max: " + adcMin.ToString() + ", " + adcMax.ToString(); }
                calibrated = true;
            }
        }

        if (calibrating)
        {
            if (sensorADC > adcMax) { adcMax = sensorADC; }
            if (sensorADC <adcMin) { adcMin = sensorADC; }

        }

        if (debug) { adcText.text = "ADC: " + sensorADC.ToString(); }

        if (debug)
        {
            if (breatheIn) { bg.color = Color.green; }
            else { bg.color = red; }
        }
    }

    public void OnMessageArrived(string msg)
    {
        sensorADC = float.Parse(msg);

        if (calibrated)
        {
            if (adcAverage.Count >= sampleFrames) { adcAverage.Dequeue(); }
            adcAverage.Enqueue(sensorADC);

            float total = 0;
            foreach (float val in adcAverage) { total += val; }
            currentSample = Mathf.Floor(total / adcAverage.Count);

            if (samples.Count > sampleFrames) { samples.Dequeue(); }
            samples.Enqueue(currentSample);

            // so it only procs once
            if (!breatheIn)
            {
                if (currentSample > samples.Peek() && currentSample-samples.Peek() >= sensThreshold) 
                { 
                    breatheIn = true; 
                    onBreatheIn.Invoke();
                    Debug.Log("breathe in");
                }
            } 
            else
            {
                if (currentSample < samples.Peek() && samples.Peek()-currentSample >= sensThreshold) 
                { 
                    breatheIn = false; onBreatheOut.Invoke();
                    Debug.Log("breathe out");
                }
            }

            //Debug.Log(currentSample + ", " + samples.Peek().ToString());

            if (debug) { avgText.text = "ADC Avg: " + currentSample.ToString(); }
        }

        lastReading = sensorADC;
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
