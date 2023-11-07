using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.AI;

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
        playerMovement.ResumeMovement();
    }
    private void Update()
    {
        if (!isCutScene && MultiScene.Instance.bossObject)
        {
            if(MultiScene.Instance.bossObject.TryGetComponent(out EnemyHealth enemyHealth))
            {
                if (enemyHealth.currentHealth <= 0)
                {
                    LastCutScene();
                }
            }
        }
    }
    private IEnumerator BossStop()
    {
        yield return new WaitForSeconds(1f);
        MultiScene.Instance.bossObject.TryGetComponent(out MultiBoss multiBoss);
        multiBoss.StopCoroutine(multiBoss.PlayerDetect());
        multiBoss.StopCoroutine(multiBoss.ChangeTarget());
        multiBoss.StopCoroutine(multiBoss.StartThink());
        Debug.LogWarning("보스 스탑 확인");
    }
    public void LastCutScene()
    {
        isCutScene = true;
        BossStop();
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
