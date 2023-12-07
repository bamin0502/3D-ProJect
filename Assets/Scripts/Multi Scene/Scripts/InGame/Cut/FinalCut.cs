using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
public class FinalCut : MonoBehaviour
{
    //실행시킬 마지막 타임라인
    public PlayableDirector playableDirector;
    public TimelineAsset lastcut;
    private bool isCutScene = false;
    public MultiPlayerMovement playerMovement;
    void Start()
    {
        MultiScene.Instance.lastPlayableDirector = playableDirector;
        playableDirector.stopped += OnlastCutsceneEnd;
    }
    private void OnlastCutsceneEnd(PlayableDirector director)
    {
        SoundManager.instance.bgmAudioSource.Play();
        playerMovement.StopMovement();
    }
    private void Update()
    {
        if (isCutScene || !MultiScene.Instance.bossObject) return;
        if (!MultiScene.Instance.bossObject.TryGetComponent(out EnemyHealth enemyHealth)) return;
        if (enemyHealth.currentHealth <= 0)
        {
            LastCutScene();
        }
    }
    public void LastCutScene()
    {
        isCutScene = true;
        playableDirector.playableAsset = lastcut;
        playableDirector.Play();
        if(playableDirector.playableAsset==lastcut)
        {
            SoundManager.instance.bgmAudioSource.Stop();
        }
        MultiScene.Instance.BroadCastingLastCutSceneStart(true);
        Debug.LogWarning("마지막 컷신 확인");
    }
}
