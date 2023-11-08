using System;
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
    public ParticleSystem deathParticle;
    public ParticleSystem deathBloodParticle;
    private static readonly int DoDie = Animator.StringToHash("doDie");
    private MultiMyStatus _multiMyStatus;
    private MultiTeamstatus _multiTeamstatus;

    void Start()
    {
        
        string json = "{\"PlayerHealth\": 10000, \"Health\": 10000}";
        var followCamera = MultiScene.Instance.playerCamera;
        PlayerStat playerStat = JsonConvert.DeserializeObject<PlayerStat>(json);
        MaxHealth = (int)playerStat.PlayerHealth;
        CurrentHealth = (int)playerStat.Health;
        _multiMyStatus = GetComponent<MultiMyStatus>();
        _multiTeamstatus = GetComponent<MultiTeamstatus>();
        MultiScene.Instance.multiPlayerHealthBar.UpdatePlayerHp();
    }
    
    public void TakeDamage(int damage)
    {
        CurrentHealth = Math.Max(CurrentHealth - damage, 0);
        if(CurrentHealth <= 0) CurrentHealth = 0;
        UpdateHealth();
        if (CurrentHealth <= 0)
        {
            if(!MultiScene.Instance.isMasterClient) return;
            Die();
            Invoke(nameof(EndDeath), 3f);
            MultiScene.Instance.BroadCastingPlayerDead(gameObject.name);
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
        gameObject.layer = 2;
        deathParticle.Play();
        deathBloodParticle.Play();
        if (MultiScene.Instance.currentUser.Equals(gameObject.name))
        {
            MultiScene.Instance.ChangeColor();
            MultiScene.Instance.isDead = true;
        }
        MultiScene.Instance._players.Remove(gameObject.name);
        GetComponent<MultiPlayerMovement>().SetAnimationTrigger(DoDie);
        deathText.DOText("당신은 "+ "<color=red>" + "몬스터"+ "</color>" + "에게 죽었습니다.", 3, true, ScrambleMode.None, null);
        endingImage.rectTransform.gameObject.SetActive(true);
        
        foreach (GameObject enemy in MultiScene.Instance.enemyList)
        {
            if(enemy == null) continue;
            
            if (enemy.TryGetComponent<MultiEnemy>(out var e))
            {
                if (e.DetectCoroutine != null) e.StopCoroutine(e.DetectCoroutine);
                if (e.AttackCoroutine != null) e.StopCoroutine(e.AttackCoroutine);
                e._targetPos = null;
                e.AttackCoroutine = e.StartCoroutine(e.PlayerDetect());
                e.AttackCoroutine = e.StartCoroutine(e.TryAttack());
            }
        }
    }
    public void EndDeath()
    {
        Destroy(gameObject);
    }
}
