using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] private Sound[] effectSounds;
    [SerializeField] private Sound[] bgmSounds;

    public AudioSource bgmAudioSource;
    [SerializeField] private AudioSource[] effectAudioSources;

    private float bgmVolume = 1f;
    private float sfxVolume = 1f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        bgmAudioSource.volume = PlayerPrefs.GetFloat("bgmVolume", 1f);
        foreach (AudioSource effectAudioSource in effectAudioSources)
        {
            effectAudioSource.volume = PlayerPrefs.GetFloat("sfxVolume", 1f);
        }
    }

    public void SetBgmVolume(float volume)
    {
        bgmVolume = volume;
        bgmAudioSource.volume = volume;
    }

    public void SetSfxVolume(float volume)
    {
        sfxVolume = volume;
        foreach (AudioSource effectAudioSource in effectAudioSources)
        {
            effectAudioSource.volume = volume;
        }
    }

    public void PlaySE(string name)
    {
        Sound effectSound = System.Array.Find(effectSounds, sound => sound.name == name);
        if (effectSound != null)
        {
            foreach (AudioSource effectAudioSource in effectAudioSources)
            {
                if (!effectAudioSource.isPlaying)
                {
                    effectAudioSource.clip = effectSound.clip;
                    effectAudioSource.Play();
                    break;
                }
            }
        }
        else
        {
            Debug.Log(name + " sound is not registered in SoundManager.");
        }
    }

    public void StopSE(string name)
    {
        Sound effectSound = System.Array.Find(effectSounds, sound => sound.name == name);
        if (effectSound != null)
        {
            foreach (AudioSource effectAudioSource in effectAudioSources)
            {
                if (effectAudioSource.isPlaying && effectAudioSource.clip == effectSound.clip)
                {
                    effectAudioSource.Stop();
                    break;
                }
            }
        }
        else
        {
            Debug.Log(name + " sound is not registered in SoundManager.");
        }
    }
}