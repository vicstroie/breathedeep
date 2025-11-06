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

    Queue<float> adcAverage = new Queue<float>();
    Queue<float> samples = new Queue<float>();
    float lastSample;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        red = bg.color;
    }

    // Update is called once per frame
    void Update()
    {
        //sensorADC = FindFirstObjectByType<SerialReader>().GetValue();
        //Debug.Log(FindFirstObjectByType<SerialController>().ReadSerialMessage());


        if (Input.GetKeyDown(startSampleKey))
        {
            calibrating = !calibrating;

            calibratingText.SetActive(calibrating);

            if (!calibrating)
            {
                minMaxText.text = "Min, Max: " + adcMin.ToString() + ", " + adcMax.ToString();
                calibrated = true;
            }
        }

        if (calibrating)
        {
            if (sensorADC > adcMax) { adcMax = sensorADC; }
            if (sensorADC <adcMin) { adcMin = sensorADC; }
        }

        adcText.text = "ADC: " + sensorADC.ToString();   

        if (breatheIn) { bg.color = Color.green; }
        else { bg.color = red; }
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
            float currentSample = Mathf.Floor(total / adcAverage.Count);

            if (samples.Count > sampleFrames) { samples.Dequeue(); }
            samples.Enqueue(currentSample);

            breatheIn = currentSample > lastSample;
            lastSample = currentSample;
            breatheIn = sensorADC > samples.Peek();
            Debug.Log(currentSample + ", " + samples.Peek().ToString());

            avgText.text = "ADC Avg: " + currentSample.ToString();
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
