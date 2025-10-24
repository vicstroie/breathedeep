using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    GameObject audioManager;
    List<Sound> sounds = new();

    public void Start()
    {
        audioManager = gameObject;

        InitSounds();

        foreach (Sound s in sounds)
        {
            s.source.resource = s.clip;
            s.source.volume = s.volume;
            s.source.loop = s.loop;
        }

        Play("Ambient Loop");
    }

    public void CreateSound(string name, string file)
    {
        Sound s = new Sound(
            source: audioManager.AddComponent<AudioSource>(),
            name: name,
            clip: Resources.Load<AudioClip>("Sounds/" + file),
            volume: 1,
            pitchMin: 1,
            pitchMax: 1,
            loop: false
        );

        sounds.Add(s);

        s.source.resource = s.clip;
        s.source.volume = s.volume;
        s.source.loop = s.loop;
    }

    public void InitSounds()
    {
        /* TEMPLATE
        sounds.Add(new Sound(
            source: audioManager.AddComponent<AudioSource>(),
            name: "",
            clip: Resources.Load<AudioClip>("Sounds/FILE NAME"),
            volume: ,
            pitchMin: ,
            pitchMax: ,
            loop: false
        ));
        */

        sounds.Add(new Sound(
            source: audioManager.AddComponent<AudioSource>(),
            name: "Ambient Loop",
            clip: Resources.Load<AudioClip>("Sounds/ambientLoop"),
            volume: 1,
            pitchMin: 1,
            pitchMax: 1,
            loop: true
        ));
    }

    public void Play(string name)
    {
        foreach (Sound s in sounds)
        {
            if (s.name == name)
            {
                s.source.pitch = Random.Range(s.pitchMin, s.pitchMax);
                s.source.Play();
            }
        }
    }

    public struct Sound
    {
        public string name;

        public AudioSource source;

        public AudioClip clip;
        public float volume;
        public float pitchMin;
        public float pitchMax;
        public bool loop;

        public Sound(AudioSource source, string name, AudioClip clip, float volume = 1, float pitchMin = 1, float pitchMax = 1, bool loop = false)
        {
            this.source = source;
            this.name = name;
            this.clip = clip;
            this.volume = Mathf.Clamp(volume, 0.1f, 1);
            this.pitchMin = Mathf.Clamp(pitchMin, 0.1f, 3);
            this.pitchMax = Mathf.Clamp(pitchMax, 0.1f, 3);
            this.loop = loop;
        }
    }
}
