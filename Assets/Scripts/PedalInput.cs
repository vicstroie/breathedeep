using UnityEngine;
using UnityEngine.Events;
using System;

public class PedalInput : MonoBehaviour
{
    [SerializeField] public KeyCode leftKey = KeyCode.K;
    [SerializeField] public KeyCode rightKey = KeyCode.L;

    [Header("Events")]
    [Space(5)]
    [SerializeField] UnityEvent onLeftDown;
    [SerializeField] UnityEvent onLeft;
    [SerializeField] UnityEvent onRightDown;
    [SerializeField] UnityEvent onRight;

    static float inputValue;

    static bool left = false;
    static bool right = false;

    public static PedalInput instance;

    public static float InputValue { get { return inputValue; } }
    public static bool Left { get { return left; } }
    public static bool Right { get { return right; } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instance == null) { instance = this; }
        else { Destroy(gameObject); }
    }

    // Update is called once per frame
    void Update()
    {
        left = Input.GetKey(leftKey);
        right = Input.GetKey(rightKey);

        if (Input.GetKeyDown(leftKey)) { onLeftDown.Invoke(); }
        if (Input.GetKeyDown(rightKey)) { onRightDown.Invoke(); }

        if (left) { onLeft.Invoke(); }
        if (right) { onRight.Invoke(); }

        inputValue = (Convert.ToInt32(left) * -1) + (Convert.ToInt32(right));
    }
}
