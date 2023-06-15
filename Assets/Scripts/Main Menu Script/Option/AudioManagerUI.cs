using UnityEngine;
using UnityEngine.UI;

public class AudioManagerUI : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider sfxSlider;

    private SoundManager soundManager;

    private void Start()
    {
        soundManager = SoundManager.instance;

        bgmSlider.value = PlayerPrefs.GetFloat("bgmVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume", 1f);

        bgmSlider.onValueChanged.AddListener(OnBgmVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
    }

    private void OnBgmVolumeChanged(float volume)
    {
        soundManager.SetBgmVolume(volume);
        PlayerPrefs.SetFloat("bgmVolume", volume);
    }

    private void OnSfxVolumeChanged(float volume)
    {
        soundManager.SetSfxVolume(volume);
        PlayerPrefs.SetFloat("sfxVolume", volume);
    }
}