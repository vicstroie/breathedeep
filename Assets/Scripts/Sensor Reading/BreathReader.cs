using NUnit.Framework;
using System.Collections;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using System.IO.Ports;
using TMPro;
using System.Linq;

public class BreathReader : MonoBehaviour
{
    bool breatheIn = false; // false = breathe out
    float sensorADC = 0; // the value returned by the analog port the stretch sensor is plugged into
    float lastReading;

    float adcMin = 10000;
    float adcMax = 0;

    [Header("Debug")]
    [SerializeField] bool debug = false;
    [SerializeField] TMP_Text adcText;
    [SerializeField] TMP_Text avgText;
    [SerializeField] TMP_Text minMaxText;
    [SerializeField] GameObject calibratingText;
    [SerializeField] TMP_Text inValueText;
    [SerializeField] Image bg;
    Color red;

    [Space(10)]
    [SerializeField] KeyCode calibrateKey = KeyCode.Space;
    bool sampling = true;
    bool calibrating = false;
    bool calibrated = true;

    [SerializeField] UnityEvent onBreatheIn;
    [SerializeField] UnityEvent onBreatheOut;
    [SerializeField] UnityEvent onStateChange;

    [Tooltip("How many frames one sample (average) is")]
    [SerializeField] int sampleLength = 15;
    [Tooltip("How many samples at a time to store")]
    [SerializeField] int sampleFrames = 15;
    [SerializeField] float sensThreshold = 5f; // should calibrate at start by having them stand still/hold their breath

    Queue<float> adcAverage = new Queue<float>(); // holds the values from the last 60 frames (or whatever the value of sampleFrames is) to average it
    Queue<float> samples = new Queue<float>();
    float currentSample;
    float lastBreath; // the value of the sample at the last breathe in

    float holdingTime = 0; // time since the last state change

    public float CurrentADC { get { return sensorADC; } }
    public float CurrentSample { get { return currentSample; } }
    public Queue<float> StoredSamples { get { return samples; } }
    public float SensThreshold
    {
        get { return sensThreshold; }
        set { sensThreshold = value; }
    }

    public float HoldingTime { get { return holdingTime; } }

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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            lastBreath = 0;
            breatheIn = false;
        }

        //if (Input.GetKeyDown(calibrateKey))
        //{
        //    calibrating = !calibrating;

        //    if (debug) { calibratingText.SetActive(calibrating); }

        //    if (!calibrating)
        //    {
        //        if (debug) { minMaxText.text = "Min, Max: " + adcMin.ToString() + ", " + adcMax.ToString(); }
        //        calibrated = true;
        //    }
        //}

        if (calibrating)
        {
            if (sensorADC > adcMax) { adcMax = sensorADC; }
            if (sensorADC < adcMin) { adcMin = sensorADC; }
        }

        if (debug) { adcText.text = "ADC: " + sensorADC.ToString(); }

        if (debug)
        {
            if (breatheIn) { bg.color = Color.green; }
            else { bg.color = red; }
        }

        holdingTime += Time.deltaTime;
    }

    public void OnMessageArrived(string msg)
    {
        sensorADC = float.Parse(msg);

        if (calibrated)
        {
            if (adcAverage.Count >= sampleLength) { adcAverage.Dequeue(); }
            adcAverage.Enqueue(sensorADC);

            float total = 0;
            float highest = 0;
            int highestIndex = 0;
            var avg = adcAverage.ToList<float>();
            for (int i = 0; i < avg.Count; i++)
            {
                float val = avg[i];
                if (val > highest)
                {
                    highest = val;
                    highestIndex = i;
                }
            }
            avg.RemoveAt(highestIndex);
            foreach (float val in avg)
            {
                total += val;
            }
            currentSample = Mathf.Floor(total / avg.Count);
            //currentSample = sensorADC;

            if (samples.Count > sampleFrames) { samples.Dequeue(); }
            samples.Enqueue(currentSample);

            // so it only procs once
            if (!breatheIn)
            {
                if (currentSample > samples.Peek() && currentSample - samples.Peek() >= sensThreshold)
                {
                    if (debug) { inValueText.text = "In value: " + currentSample.ToString(); }
                    breatheIn = true;
                    lastBreath = currentSample;
                    onBreatheIn.Invoke();
                    onStateChange.Invoke();
                    //Debug.Log("breathe in");
                    holdingTime = 0;
                }
            }
            else
            {
                if (currentSample < lastBreath)
                {
                    breatheIn = false; onBreatheOut.Invoke();
                    onStateChange.Invoke();
                    //Debug.Log("breathe out");
                    holdingTime = 0;
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