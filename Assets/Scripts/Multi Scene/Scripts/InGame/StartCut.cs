using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class StartCut : MonoBehaviour
{
    public PlayableDirector _playableDirector;
    public TimelineAsset FirstCut;
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
    }
    // 컷신 종료 시 호출될 이벤트 핸들러
    public void OnCutsceneEnd(PlayableDirector director)
    {
        // BGM을 다시 재생
        SoundManager.instance.bgmAudioSource.Play();
    }
}
