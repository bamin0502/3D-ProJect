using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class StartCut : MonoBehaviour
{
    public PlayableDirector _playableDirector;
    public TimelineAsset FirstCut;
    public ParticleSystem BossEffect;
    private void Awake()
    {
        _playableDirector = GetComponent<PlayableDirector>();
    }
    void Start()
    {
        if (_playableDirector.playableAsset == FirstCut)
        {
            // 컷신이 시작되면 BGM을 중지
            SoundManager.instance.bgmAudioSource.Stop();

        }

        // 컷신 종료 이벤트 핸들러 등록
        _playableDirector.stopped += OnCutsceneEnd;
        _playableDirector.paused += PlayEffect;
        _playableDirector.played += StopEffect;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 'Esc' 키를 누르면 컷신을 넘깁니다.
            _playableDirector.time = _playableDirector.duration;
        }  
    }
    // 컷신 종료 시 호출될 이벤트 핸들러
    public void OnCutsceneEnd(PlayableDirector director)
    {
        // BGM을 다시 재생
        SoundManager.instance.bgmAudioSource.Play();
    }

    public void PlayEffect(PlayableDirector director)
    {
        BossEffect.Play();
    }
    public void StopEffect(PlayableDirector director)
    {
        BossEffect.Stop();
    }
}
