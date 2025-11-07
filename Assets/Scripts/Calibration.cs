using UnityEngine;

public class Calibration : MonoBehaviour
{
    public enum CALIB_STATE { MIN_MAX, SENS };

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
