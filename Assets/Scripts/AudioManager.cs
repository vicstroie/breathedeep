using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; //singleton instance

    [SerializeField] FMODUnity.EventReference playerWalk;
    [SerializeField] FMODUnity.EventReference ambience;
    [SerializeField] FMODUnity.EventReference keyPickup;
    [SerializeField] FMODUnity.EventReference monsterSpawn;
    [SerializeField] FMODUnity.EventReference monsterAmbience;
    [SerializeField] FMODUnity.EventReference playerHit;

    FMOD.Studio.EventInstance monsterAmbienceInstance;

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
        monsterAmbienceInstance = FMODUnity.RuntimeManager.CreateInstance(monsterAmbience);

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

    public void PlayMonsterAmbience()
    {
        monsterAmbienceInstance.start();
    }

    public void SetMonsterTransform(Transform transform)
    {
        monsterAmbienceInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform));
    }

    public void PlayMonsterSpawn()
    {
        FMODUnity.RuntimeManager.PlayOneShot(monsterSpawn);
    }

    public void PlayPlayerHit()
    {
        FMODUnity.RuntimeManager.PlayOneShot(playerHit);
    }

    public void PlayKeyPickup()
    {
        FMODUnity.RuntimeManager.PlayOneShot(keyPickup);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
