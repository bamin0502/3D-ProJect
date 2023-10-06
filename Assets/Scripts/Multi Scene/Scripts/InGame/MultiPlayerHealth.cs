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
    public static int MaxHealth;
    public static int CurrentHealth;
    public PlayerHealthBar healthBar;
    public TMP_Text deathText;
    public Image endingImage;

    public Image killedImage;
    public TMP_Text kill;
    public TMP_Text death;
    public ParticleSystem deathParticle;
    public ParticleSystem deathBloodParticle;

    private static readonly int DoDie = Animator.StringToHash("doDie");

    // Start is called before the first frame update
    void Start()
    {
        string json = "{\"PlayerHealth\": 10000, \"Health\": 10000}";
        PlayerStat playerStat = JsonConvert.DeserializeObject<PlayerStat>(json);
        MaxHealth = (int)playerStat.PlayerHealth;
        CurrentHealth = (int)playerStat.Health;
        FindObjectOfType<PlayerHealthBar>().UpdatePlayerHp();
    }

    void Awake()
    {
        healthBar = GameObject.FindGameObjectWithTag("PlayerHealthBar").GetComponent<PlayerHealthBar>();
        deathText = GameObject.FindGameObjectWithTag("DeathText").GetComponent<TMP_Text>();
        endingImage = GameObject.FindGameObjectWithTag("Ending Image").GetComponent<Image>();
        death = GameObject.FindGameObjectWithTag("Death").GetComponent<TMP_Text>();
        killedImage = GameObject.FindGameObjectWithTag("Killed Image").GetComponent<Image>();
        kill = GameObject.FindGameObjectWithTag("Kill").GetComponent<TMP_Text>();
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        healthBar.UpdatePlayerHp();
        
        if (CurrentHealth <= 0)
        {
            Die();
            StartCoroutine(DeathTitle());
        }
    }

    public void Die()
    {
        deathParticle.Play();
        deathBloodParticle.Play(); 
        MultiScene.Instance.BroadCastingAnimation((int)PlayerState.SpaceMove);
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
