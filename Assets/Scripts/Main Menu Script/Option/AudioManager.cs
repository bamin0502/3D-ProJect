using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Inst;

    [Range(0f, 1f)]
    public float bgmVolume = 0.5f;

    [Range(0f, 1f)]
    public float sfxVolume = 0.5f;

    private AudioSource bgmAudioSource;
    private AudioSource[] sfxAudioSource;
    private void Awake()
    {
        if (Inst == null)
        {
            Inst = this;
            DontDestroyOnLoad(gameObject);

            // AudioSource 컴포넌트 추가
            bgmAudioSource = gameObject.AddComponent<AudioSource>();
            sfxAudioSource = new AudioSource[10];

            for(int i=0; i<sfxAudioSource.Length; i++)
            {
                sfxAudioSource[i] = gameObject.AddComponent<AudioSource>();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBGM(AudioClip clip)
    {
        // BGM 재생 코드
        bgmAudioSource.clip = clip;
        bgmAudioSource.volume = bgmVolume;
        bgmAudioSource.loop = true;
        bgmAudioSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        // SFX 재생 코드
        AudioSource availableSource = GetAvailabelSFXAudioSource();
        if (availableSource != null)
        {
            availableSource.PlayOneShot(clip, sfxVolume);
        }

        
    }
    private AudioSource GetAvailabelSFXAudioSource()
    {
        for(int i=0; i<sfxAudioSource.Length; i++)
        {
            if (!sfxAudioSource[i].isPlaying)
            {
                return sfxAudioSource[i];
            }
            
        }
        return null;
    }
}