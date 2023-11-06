using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.AI;
public class EnemyCut : MonoBehaviour
{
    //실행시킬 2번째 타임라인
    public PlayableDirector playableDirector;
    public TimelineAsset SecondCut;
    //해당 오브젝트에 자식이 있는지 없는지 검사시킬 오브젝트
    public GameObject checkObject;
    private bool isCutScene = false;
    public MultiPlayerMovement playerMovement;

    void Start()
    {
        checkObject = MultiScene.Instance.Enemy;
        MultiScene.Instance.secondPlayableDirector = playableDirector;
        
        playableDirector.stopped += OnCutsceneEnd;
    }

    private void Awake()
    {
        playableDirector = GetComponent<PlayableDirector>();
        
    }

    private void OnCutsceneEnd(PlayableDirector director)
    {
        // BGM을 다시 재생
        SoundManager.instance.bgmAudioSource.Play();
        StartCoroutine(bossCutScene());
        playerMovement.ResumeMovement();
    }
    void Update()
    {
        StartSecondScene();
    }
    public void StartSecondScene()
    {
        if (checkObject.transform.childCount == 0 && !isCutScene)
        {
            isCutScene = true;
            MultiScene.Instance.nav.areaMask=NavMesh.AllAreas;
            playableDirector.playableAsset = SecondCut;
            playableDirector.Play();
            playerMovement.StopMovement();
            if (playableDirector.playableAsset == SecondCut)
            {
                // 컷신이 시작되면 BGM을 중지
                SoundManager.instance.bgmAudioSource.Stop();
            }
            
            MultiScene.Instance.BroadCastingSecondCutSceneStart(true);
            Debug.LogWarning("컷신 나오는지 확인용");
        }
    }
    IEnumerator bossCutScene()
    {
        yield return new WaitForSeconds(1f);
        MultiScene.Instance.bossObject.TryGetComponent(out MultiBoss multiBoss);
        if (multiBoss != null)
        {
            Debug.LogWarning("Setting");
            multiBoss.StartCoroutine(multiBoss.PlayerDetect());
            multiBoss.StartCoroutine(multiBoss.ChangeTarget());
            multiBoss.StartCoroutine(multiBoss.StartThink());
        }
    }
}
