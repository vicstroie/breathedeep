using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; //singleton instance

    [SerializeField] FMODUnity.EventReference playerWalk;
    [SerializeField] FMODUnity.EventReference ambience;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(this);
        }

        DontDestroyOnLoad(this);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayAmbience();
    }

    public void PlayAmbience()
    {
        FMODUnity.RuntimeManager.PlayOneShot(ambience);
    }

    public void PlayFootsteps()
    {
        FMODUnity.RuntimeManager.PlayOneShot(playerWalk);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
