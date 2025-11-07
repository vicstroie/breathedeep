using UnityEngine;
using UnityEngine.UI;

public class TestMover : MonoBehaviour
{

    Vector3 targetPos;
    RectTransform rectTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        targetPos = rectTransform.anchoredPosition;
    }

    // Update is called once per frame
    void Update()
    {
        rectTransform.anchoredPosition = Vector3.Lerp(rectTransform.anchoredPosition, targetPos, 10 * Time.deltaTime);
    }

    public void MoveSpace()
    {
        targetPos += new Vector3(50, 0, 0);
    }
}
