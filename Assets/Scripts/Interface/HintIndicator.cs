using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HintIndicator : MonoBehaviour
{
    [SerializeField] Image fillImage;
    [SerializeField] float defaultAlpha = 0.65f;

    Image bgImage;

    bool showing = false;

    [HideInInspector] public float fillValue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bgImage = GetComponent<Image>();

        Color bgColor = bgImage.color;
        Color fillColor = fillImage.color;
        bgImage.color = new Color(bgColor.r, bgColor.g, bgColor.b, 0);
        fillImage.color = new Color(fillColor.r, fillColor.g, fillColor.b, 0);
    }

    // Update is called once per frame
    void Update()
    {
        fillImage.fillAmount = fillValue;
    }

    public void SetShowing(bool _showing)
    {
        if (_showing != showing)
        {
            showing = _showing;
            if (showing)
            {
                StartCoroutine(FadeIn(0.75f));
            }
            else
            {
                StartCoroutine(FadeOut(0.75f));
            }
        }
    }

    IEnumerator FadeIn(float fadeSpeed)
    {
        float a = 0;

        Color bgColor = bgImage.color;
        Color fillColor = fillImage.color;
        
        while (a < defaultAlpha)
        {
            a += fadeSpeed * Time.deltaTime;
            bgImage.color = new Color(bgColor.r, bgColor.g, bgColor.b, a);
            fillImage.color = new Color(fillColor.r, fillColor.g, fillColor.b, a);
            yield return null;
        }
        bgImage.color = new Color(bgColor.r, bgColor.g, bgColor.b, defaultAlpha);
        fillImage.color = new Color(fillColor.r, fillColor.g, fillColor.b, defaultAlpha);
    }

    IEnumerator FadeOut(float fadeSpeed)
    {
        float a = defaultAlpha;

        Color bgColor = bgImage.color;
        Color fillColor = fillImage.color;

        while (a > 0)
        {
            a -= fadeSpeed * Time.deltaTime;
            bgImage.color = new Color(bgColor.r, bgColor.g, bgColor.b, a);
            fillImage.color = new Color(fillColor.r, fillColor.g, fillColor.b, a);
            yield return null;
        }
        bgImage.color = new Color(bgColor.r, bgColor.g, bgColor.b, 0);
        fillImage.color = new Color(fillColor.r, fillColor.g, fillColor.b, 0);
    }
}
