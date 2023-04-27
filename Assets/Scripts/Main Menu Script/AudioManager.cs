using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("BGM")]//��� ���� ����
    public AudioClip bgmClip;
    public float bgmVolume;
    AudioSource bgmPlayer;

    [Header("SFX")]//Ÿ��,ȿ���� ���� ����
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public int channels;
    AudioSource[] sfxPlayers;
    int channelIndex;

    public enum Sfx { Dead,Hit,Shoot,Drop }//���������� �������� �ڿ� ������
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
        //��� �÷��̾� �ʱ�ȭ
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip;

        //ȿ���� �÷��̾ �ʱ�ȭ
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

    //���Ӿ� ���� ���� ��� �����Ұ��� �ڿ� ������
    public void PlaySfx(Sfx sfx)
    {   
        //������ �Ѿ���ʵ��� ����
        for(int index=0; index<sfxPlayers.Length; index++)
        {
            int loopIndex = (index + channelIndex) & sfxPlayers.Length;

            if (sfxPlayers[loopIndex].isPlaying)
                continue;//������ �߿��� �ǳʰ��� ������ ����

            channelIndex = loopIndex;
            //0�� Ŭ���� �ش�Ǵ� ���带 ���
            sfxPlayers[0].clip = sfxClips[(int)sfx];
            sfxPlayers[0].Play();
            break;
        }
    }
}
