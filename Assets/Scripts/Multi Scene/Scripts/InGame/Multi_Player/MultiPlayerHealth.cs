using System.Collections;
using UnityEngine;
using Data;
using Newtonsoft.Json;
using TMPro;
using DG.Tweening;
using mino;
using UnityEngine.UI;

public class MultiPlayerHealth : MonoBehaviour
{
    public int MaxHealth;
    public int CurrentHealth;
   
    public TMP_Text deathText;
    [SerializeField]
    public Image endingImage;

    public Image killedImage;
    public TMP_Text kill;
    public TMP_Text death;
    public ParticleSystem deathParticle;
    public ParticleSystem deathBloodParticle;

    private static readonly int DoDie = Animator.StringToHash("doDie");

    private MultiMyStatus _multiMyStatus;
    private MultiTeamstatus _multiTeamstatus;
    // Start is called before the first frame update
    void Start()
    {
        string json = "{\"PlayerHealth\": 100, \"Health\": 100}";
        PlayerStat playerStat = JsonConvert.DeserializeObject<PlayerStat>(json);
        MaxHealth = (int)playerStat.PlayerHealth;
        CurrentHealth = (int)playerStat.Health;
        _multiMyStatus = GetComponent<MultiMyStatus>();
        _multiTeamstatus = GetComponent<MultiTeamstatus>();
        MultiScene.Instance.multiPlayerHealthBar.UpdatePlayerHp();
    }
    
    void Awake()
    {
        //endingImage.rectTransform.gameObject.SetActive(false);
        //killedImage.rectTransform.gameObject.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        if(CurrentHealth <= 0)
        {
            CurrentHealth = 0;
        }
        //자기 머리위에 보일 체력 상태창
        _multiMyStatus.UpdatePlayerHp();
        //팀 체력 상태창      
        _multiTeamstatus.UpdatePlayerHp();
        //자신 체력 메인 UI 체력상태창
        MultiScene.Instance.multiPlayerHealthBar.UpdatePlayerHp();
        CinemachineShake.Instance.ShakeCamera(4f, 1f);
        if (CurrentHealth <= 0)
        {
            Die();
            //StartCoroutine(DeathTitle());
            Invoke(nameof(EndDeath), 3f);
            
        }

    }

    public void UpdateHealth()
    {
        _multiMyStatus.UpdatePlayerHp();
        _multiTeamstatus.UpdatePlayerHp();
        MultiScene.Instance.multiPlayerHealthBar.UpdatePlayerHp();
    }

    public void Die()
    {
        if(MultiScene.Instance.isDead) return;
        
        deathParticle.Play();
        deathBloodParticle.Play();
        MultiScene.Instance.isDead = true;
        MultiScene.Instance._players.Remove(gameObject.name);
        GetComponent<MultiPlayerMovement>().SetAnimationTrigger(DoDie);
        deathText.DOText("당신은 "+ "<color=red>" + "몬스터"+ "</color>" + "에게 죽었습니다.", 3, true, ScrambleMode.None, null);
        endingImage.rectTransform.gameObject.SetActive(true);
    }
    IEnumerator DeathTitle()
    {
        killedImage.rectTransform.gameObject.SetActive(true);
        kill.text = "Monster";
        death.text = "Player";
       
        yield return new WaitForSeconds(10f);
                
        killedImage.DOFade(0f, 1f).OnComplete(() =>
        {
            killedImage.rectTransform.gameObject.SetActive(false);
        });

    }
    void EndDeath()
    {
        Destroy(gameObject);
    }
}
