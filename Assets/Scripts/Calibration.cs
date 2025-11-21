using UnityEngine;
using UnityEngine.UI;

public class Calibration : MonoBehaviour
{
    [Header("Strap Adjustment")]
    [SerializeField] float idleRange = 850;
    [SerializeField] float idleLower = 750;
    [SerializeField] float idleUpper = 950;
    float idleNormalized = 0;

    [Header("UI")]
    [SerializeField] RectTransform slider;
    [SerializeField] float sliderBound;

    [SerializeField] float test = 850;
     
    public enum CALIB_STATE { STRAP_ADJUST, MIN_MAX, SENS };

    // STRAP_ADJUST: guide the player to get the right placement of the physical strap
    // MIN_MAX: 
    // SENS: how sensitive it is to stretching, have the player hold their breath/stay still to see how much the value changes

    CALIB_STATE state = CALIB_STATE.STRAP_ADJUST;

    BreathReader reader;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        reader = FindFirstObjectByType<BreathReader>();        
    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            case CALIB_STATE.STRAP_ADJUST:
                StrapAdjustment();
                break;
            case CALIB_STATE.MIN_MAX:
                break;
            case CALIB_STATE.SENS:
                break;
        }        
    }

    void StrapAdjustment()
    {
        float sample = reader.CurrentSample;
        //float sample = test;

        idleNormalized = (sample - idleLower) / (idleUpper - idleLower);
        float xPos = -sliderBound + (sliderBound * 2 * idleNormalized);
        slider.anchoredPosition = new Vector2(xPos, slider.anchoredPosition.y);
    }
}
