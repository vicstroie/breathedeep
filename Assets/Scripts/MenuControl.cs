using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuControl : MonoBehaviour
{
    [SerializeField] RectTransform[] buttonTransforms;
    [SerializeField] RectTransform selectorArrow;

    [SerializeField] Image holdBar;
    float barValue;

    [SerializeField] float selectHoldTime = 2.5f;
    float minHoldTime = 0.25f;

    int buttonIndex = 0;
    bool acceptingInput = true;

    float selectTimer = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        holdBar.fillAmount = barValue;
        if (!acceptingInput) { return; }

        if (selectTimer == 0) { barValue = Mathf.Lerp(barValue, 0, 10 * Time.deltaTime); }
        selectTimer = Mathf.Max(0, selectTimer);

        if (PedalInput.Left || PedalInput.Right)
        {
            selectTimer += Time.deltaTime;
        
            if (selectTimer >= minHoldTime)
            {
                // start visualizing press
                float rate = 1 / (selectHoldTime - minHoldTime);
                barValue += rate * Time.deltaTime;
            }
            if (selectTimer >= selectHoldTime)
            {
                if (buttonIndex == 0)
                {
                    // play game
                    SceneLoader.instance.FadeOutAndLoad(1);
                    acceptingInput = false;
                }
                else if (buttonIndex == 1)
                {
                    // credits
                }
            }
        }
        if (Input.GetKeyUp(PedalInput.instance.leftKey) || Input.GetKeyUp(PedalInput.instance.rightKey))
        {
            if (SceneManager.GetActiveScene().name == "Menu")
            {
                if (selectTimer < minHoldTime)
                {
                    //SelectNextButton();
                }
            }
            selectTimer = 0;
        }
    }

    public void SelectNextButton()
    {
        if (buttonIndex == 0) { buttonIndex = 1; }
        else { buttonIndex = 0; }

        float _x = selectorArrow.anchoredPosition.x;
        selectorArrow.anchoredPosition = new Vector2(_x, buttonTransforms[buttonIndex].anchoredPosition.y);
    }
}
