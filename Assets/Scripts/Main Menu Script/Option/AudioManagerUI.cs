using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
public class AudioManagerUI : SerializedMonoBehaviour
{
    public Slider bgmSlider;
    public Slider sfxSlider;

    private void Start()
    {
        bgmSlider.value = PlayerPrefs.GetFloat("bgmVolume", 0.5f);
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume", 0.5f);

        AudioManager.Inst.bgmVolume = bgmSlider.value;
        AudioManager.Inst.sfxVolume = sfxSlider.value;

        bgmSlider.onValueChanged.AddListener(delegate { OnBgmVolumeChanged(); });
        sfxSlider.onValueChanged.AddListener(delegate { OnSfxVolumeChanged(); });
    }

    private void OnBgmVolumeChanged()
    {
        AudioManager.Inst.bgmVolume = bgmSlider.value;
        PlayerPrefs.SetFloat("bgmVolume", bgmSlider.value);
    }

    private void OnSfxVolumeChanged()
    {
        AudioManager.Inst.sfxVolume = sfxSlider.value;
        PlayerPrefs.SetFloat("sfxVolume", sfxSlider.value);
    }
}