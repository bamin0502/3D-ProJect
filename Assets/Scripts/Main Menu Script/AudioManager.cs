using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("BGM")]//브금 관련 설정
    public AudioClip bgmClip;
    public float bgmVolume;
    AudioSource bgmPlayer;

    [Header("SFX")]//타격,효과음 관련 설정
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public int channels;
    AudioSource[] sfxPlayers;
    int channelIndex;

    public enum Sfx { Dead,Hit,Shoot,Drop }//열거형으로 지정해줌 뒤에 쓸거임
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }
    private void Awake()
    {
        instance = this;
        Init();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    void Init()
    {
        //브금 플레이어 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip;

        //효과음 플레이어를 초기화
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];

        for(int index=0; index<sfxPlayers.Length; index++)
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;
            sfxPlayers[index].volume = sfxVolume;
        }
    }

    //게임씬 내에 넣을 브금 지정할거임 뒤에 쓸거임
    public void PlaySfx(Sfx sfx)
    {   
        //루프를 넘어가지않도록 관리
        for(int index=0; index<sfxPlayers.Length; index++)
        {
            int loopIndex = (index + channelIndex) & sfxPlayers.Length;

            if (sfxPlayers[loopIndex].isPlaying)
                continue;//루프물 중에서 건너가는 구조를 만듬

            channelIndex = loopIndex;
            //0번 클립에 해당되는 사운드를 재생
            sfxPlayers[0].clip = sfxClips[(int)sfx];
            sfxPlayers[0].Play();
            break;
        }
    }
}
