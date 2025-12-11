using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] Image blackout;
    [SerializeField] float fadeSpeed = 0.25f;

    float calibrationSens;

    public static SceneLoader instance;
    public float CalibrationSens
    {
        get { return instance.calibrationSens; }
        set {  instance.calibrationSens = value; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FadeOutAndLoad(int index)
    {
        StartCoroutine(FadeOut(index));
    }

    IEnumerator FadeOut(int index)
    {
        float a = 0;
        Color c = blackout.color;
        blackout.color = new Color(c.r, c.g, c.b, a);

        while (a < 1)
        {
            a += fadeSpeed * Time.deltaTime;
            blackout.color = new Color(c.r, c.g, c.b, a);
            yield return null;
        }
        blackout.color = new Color(c.r, c.g, c.b, 1);
        SceneManager.LoadScene(index);

        while (a > 0)
        {
            a -= fadeSpeed * Time.deltaTime;
            blackout.color = new Color(c.r, c.g, c.b, a);
            yield return null;
        }
        blackout.color = new Color(c.r, c.g, c.b, 0);
        if (index == 2)
        {
            FindFirstObjectByType<BreathReader>().SensThreshold = calibrationSens;
        }
    }
}
