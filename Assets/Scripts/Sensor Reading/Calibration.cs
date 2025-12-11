using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Calibration : MonoBehaviour
{
    [Header("Strap Adjustment")]
    [SerializeField] float idleRange = 850;
    [SerializeField] float idleBounds = 100;
    [SerializeField] float adjustTimeToHold = 5;
    float idleLower;
    float idleUpper;
    float barLower;
    float barUpper;
    float idleNormalized = 0;
    float adjustTimer = 0;

    [Header("Max")]
    [SerializeField] float maxTimeToHold = 4f;
    bool readingMax = false;
    float sampleMax = 0;
    float sampleMin = 10000;
    float maxTimer = 0;

    [Header("UI")]
    [SerializeField] RectTransform slider;
    [SerializeField] RectTransform timerImage;
    [SerializeField] float sliderBound;
    [SerializeField] TMP_Text instructions;
    float timerImageScaleSpeed = 0;

    bool transitioning = false;
    
    public enum CALIB_STATE { STRAP_ADJUST, MAX, SENS };

    // STRAP_ADJUST: guide the player to get the right placement of the physical strap
    // MAX: the max value hit when holding a breath in
    // SENS: how sensitive it is to stretching, have the player hold their breath/stay still to see how much the value changes

    CALIB_STATE state = CALIB_STATE.STRAP_ADJUST;

    BreathReader reader;

    [Header("Sensitivity Adjustment")]
    [SerializeField] Vector2 sensBounds = new Vector2(10, 28);
    [SerializeField] float adjustmentSpeed = 0.5f;
    [SerializeField] RectTransform sensSlider;
    float defaultSens;
    float currentSens;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        reader = FindFirstObjectByType<BreathReader>();
        idleLower = idleRange - idleBounds;
        idleUpper = idleRange + idleBounds;
        barLower = idleLower - idleBounds;
        barUpper = idleUpper + idleBounds;

        StartCoroutine(FadeText(false, "adjust the tightness of the harness", 1));
        timerImageScaleSpeed = 1 / adjustTimeToHold;

        defaultSens = reader.SensThreshold;
        currentSens = defaultSens;

        float sensNormalized = 1 - ((currentSens - sensBounds.x) / (sensBounds.y - sensBounds.x)); // subtract from one to flip around
        float xPos = -275 + (275 * 2 * sensNormalized);
        sensSlider.anchoredPosition = new Vector2(xPos, slider.anchoredPosition.y);
    }

    // Update is called once per frame
    void Update()
    {
        StrapAdjustment();

        if (Mathf.Abs(PedalInput.InputValue) > 0)
        {
            currentSens -= PedalInput.InputValue * Time.deltaTime;
            reader.SensThreshold = currentSens;

            float sensNormalized = 1 - ((currentSens - sensBounds.x) / (sensBounds.y - sensBounds.x)); // subtract from one to flip around
            float xPos = -275 + (275 * 2 * sensNormalized);
            sensSlider.anchoredPosition = new Vector2(xPos, slider.anchoredPosition.y);
        }
       
        if (Input.GetKeyDown(KeyCode.Return))
        {
            LoadGame();
        }

        //switch(state)
        //{
        //    case CALIB_STATE.STRAP_ADJUST:
        //        StrapAdjustment();
        //        break;
        //    case CALIB_STATE.MAX:
        //        MinMax();
        //        break;
        //    case CALIB_STATE.SENS:
        //        break;
        //}        
    }

    void LoadGame()
    {
        SceneLoader.instance.CalibrationSens = currentSens;
        SceneLoader.instance.FadeOutAndLoad(2);
    }

    void NextState()
    {
        timerImage.localScale = Vector3.zero;
        switch (state)
        {
            case CALIB_STATE.STRAP_ADJUST:
                StartCoroutine(FadeText(true, "take a deep breath", 0.5f));
                state = CALIB_STATE.MAX;
                timerImageScaleSpeed = 1 / maxTimeToHold;
                break;
            case CALIB_STATE.MAX:
                break;
            case CALIB_STATE.SENS:
                break;
        }
    }

    IEnumerator FadeText(bool inOut, string newText, float speed)
    {
        transitioning = true;
        Color textColor = instructions.color;
        float a = 1;
        if (inOut)
        {
            while (a > 0)
            {
                a -= speed * Time.deltaTime;
                instructions.color = new Color(textColor.r, textColor.g, textColor.b, a);
                yield return null;
            }
        }
        a = 0;
        instructions.color = new Color(textColor.r, textColor.g, textColor.b, a);
        instructions.text = newText;
        while (a < 1)
        {
            a += speed * Time.deltaTime;
            instructions.color = new Color(textColor.r, textColor.g, textColor.b, a);
            yield return null;
        }
        a = 1;
        instructions.color = new Color(textColor.r, textColor.g, textColor.b, a);
        transitioning = false;
    }

    void StrapAdjustment()
    {
        if (transitioning) { return; }

        float sample = reader.CurrentSample;

        idleNormalized = (sample - barLower) / (barUpper - barLower);
        float xPos = -sliderBound + (sliderBound * 2 * idleNormalized);
        slider.anchoredPosition = new Vector2(xPos, slider.anchoredPosition.y);



        //if (sample > idleLower && sample < idleUpper)
        //{
        //    adjustTimer += Time.deltaTime;
        //    timerImage.localScale += new Vector3(timerImageScaleSpeed * Time.deltaTime, timerImageScaleSpeed * Time.deltaTime, timerImageScaleSpeed * Time.deltaTime);
        //}
        //else
        //{
        //    adjustTimer = 0;
        //    timerImage.localScale = Vector3.zero;
        //}

        //if (adjustTimer >= adjustTimeToHold)
        //{
        //    NextState();
        //}

        if (sample < sampleMin) { sampleMin = sample; }
    }

    void MinMax()
    {
        if (transitioning) { return; }

        if (readingMax)
        {
            float sample = reader.CurrentSample;

            if (sample > sampleMax) { sampleMax = sample; }
            maxTimer += Time.deltaTime;
            timerImage.localScale += new Vector3(timerImageScaleSpeed * Time.deltaTime, timerImageScaleSpeed * Time.deltaTime, timerImageScaleSpeed * Time.deltaTime);
        }
    }

    public void StartReadingMax()
    {
        if (transitioning) { return; }

        if (state == CALIB_STATE.MAX && !readingMax)
        {
            readingMax = true;
            StartCoroutine(FadeText(true, "hold your breath", 1.5f));
            transitioning = false;
        }
    }

    public void StopReadingMax()
    {
        if (maxTimer < maxTimeToHold)
        {
            readingMax = false;
            sampleMax = 0;
            timerImage.localScale = Vector3.zero;
        }
        else
        {
            readingMax = false;
            float diff = sampleMax - sampleMin;
            Debug.Log(sampleMin.ToString() + ", " + sampleMax.ToString());
        }
    }
}
