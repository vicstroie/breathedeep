using UnityEngine;

public class CameraControl : MonoBehaviour
{
    // screenshake
    float shakeTimer;
    float shakeAmount;
    bool shaking = false;
    bool damping = false;
    float dampRate;

    public static CameraControl instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instance == null) { instance = this; }
        else { Destroy(this); }
    }

    // Update is called once per frame
    void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
        }
        else
        {
            shakeTimer = 0;
            shaking = false;
            transform.localPosition = Vector3.zero;
        }

        if (shaking)
        {
            transform.localPosition = Random.insideUnitSphere * shakeAmount;
            if (damping)
            {
                shakeAmount -= dampRate * Time.deltaTime;
            }
        }
    }

    public void ScreenShake(float time, float amount, bool _damping = false)
    {
        shaking = true;
        shakeTimer = time;
        shakeAmount = amount;

        damping = _damping;
        if (damping)
        {
            dampRate = amount / time;
        }
    }
}
