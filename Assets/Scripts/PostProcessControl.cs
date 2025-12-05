using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessControl : MonoBehaviour
{
    Volume volume;

    ChromaticAberration aberration;
    [HideInInspector] public float aberrationIntesity = 0;

    Vignette vignette;
    [SerializeField] public float defaultVignette = 0.2f;
    [HideInInspector] public float vignetteIntensity = 0.2f;

    public static PostProcessControl instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instance == null) { instance = this; }
        else { Destroy(this); }

        volume = GetComponent<Volume>();
        volume.profile.TryGet(out aberration);
        volume.profile.TryGet(out vignette);
    }

    // Update is called once per frame
    void Update()
    {
        aberration.intensity.value = aberrationIntesity;
        vignette.intensity.value = vignetteIntensity;
    }
}
