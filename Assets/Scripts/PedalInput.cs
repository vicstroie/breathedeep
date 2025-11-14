using UnityEngine;
using UnityEngine.Events;
using System;

public class PedalInput : MonoBehaviour
{
    [SerializeField] KeyCode leftKey = KeyCode.K;
    [SerializeField] KeyCode rightKey = KeyCode.L;

    [Header("Events")]
    [Space(5)]
    [SerializeField] UnityEvent onLeftDown;
    [SerializeField] UnityEvent onLeft;
    [SerializeField] UnityEvent onRightDown;
    [SerializeField] UnityEvent onRight;

    static float inputValue;

    static bool leftDown = false;
    static bool rightDown = false;

    //public static PedalInput instance;

    public static float InputValue { get { return inputValue; } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //if (instance == null) { instance = this; }
        //else { Destroy(gameObject); }
    }

    // Update is called once per frame
    void Update()
    {
        leftDown = Input.GetKey(leftKey);
        rightDown = Input.GetKey(rightKey);

        if (Input.GetKeyDown(leftKey)) { onLeftDown.Invoke(); }
        if (Input.GetKeyDown(rightKey)) { onRightDown.Invoke(); }

        if (leftDown) { onLeft.Invoke(); }
        if (rightDown) { onRight.Invoke(); }

        inputValue = (Convert.ToInt32(leftDown) * -1) + (Convert.ToInt32(rightDown));
    }
}
