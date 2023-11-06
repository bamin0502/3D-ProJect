using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Cinemachine;
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 'Esc' 키를 누르면 컷신을 넘깁니다.
            _playableDirector.time = _playableDirector.duration;
            StartCoroutine(SetEnemy());
        }  
    }

    private IEnumerator SetEnemy()
    {
        yield return new WaitForSeconds(1f);

        foreach (GameObject enemy in MultiScene.Instance.enemyList)
        {
            if (enemy.TryGetComponent<MultiEnemy>(out var e))
            {
                e.DetectCoroutine = e.StartCoroutine(e.PlayerDetect());
                e.AttackCoroutine = e.StartCoroutine(e.TryAttack());
                e.SetIndex();
            }
            else
            {
                Debug.LogWarning("MultiEnemy 컴포넌트를 찾을 수 없습니다. GameObject 이름: " + enemy.name);
            }
        }
        // MultiScene.Instance.bossObject.TryGetComponent(out MultiBoss multiBoss);
        // if (multiBoss != null)
        // {
        //     Debug.LogWarning("Setting");
        //     multiBoss.StartCoroutine(multiBoss.PlayerDetect());
        //     multiBoss.StartCoroutine(multiBoss.ChangeTarget());
        //     multiBoss.StartCoroutine(multiBoss.StartThink());
        // }

        MultiScene.Instance._players.TryGetValue(MultiScene.Instance.currentUser, out GameObject player);
        if (player != null)
        {
            var weaponController = player.GetComponent<MultiWeaponController>();
            if (weaponController != null)
            {
                weaponController.StartCoroutine(weaponController.CheckCanPickupWeapon());
            }
        }
    }

    // 컷신 종료 시 호출될 이벤트 핸들러
    private void OnCutsceneEnd(PlayableDirector director)
    {
        // BGM을 다시 재생
        SoundManager.instance.bgmAudioSource.Play();
        StartCoroutine(SetEnemy());
    }
}
