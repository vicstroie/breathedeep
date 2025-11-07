using UnityEngine;

public class Calibration : MonoBehaviour
{
    public enum CALIB_STATE { MIN_MAX, SENS };

    // MIN_MAX: 
    // SENS: how sensitive it is to stretching, have the player hold their breath/stay still to see how much the value changes


    CALIB_STATE state;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            case CALIB_STATE.MIN_MAX:
                break;
            case CALIB_STATE.SENS:
                break;
        }        
    }
}
