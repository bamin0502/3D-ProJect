using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using Newtonsoft.Json;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public static int maxHealth;
    public static int currentHealth;
    public PlayerHealthBar HealthBar;
    public Animator anim;
    public TMP_Text deathText;
    public Image EndingImage;

    public Image KilledImage;
    public TMP_Text kill;
    public TMP_Text death;

    // Start is called before the first frame update
    void Start()
    {
        string json = "{\"PlayerHealth\": 1000, \"Health\": 1000}";
        PlayerStat playerStat = JsonConvert.DeserializeObject<PlayerStat>(json);
        maxHealth = (int)playerStat.PlayerHealth;
        currentHealth = (int)playerStat.Health;
        FindObjectOfType<PlayerHealthBar>().UpdatePlayerHp();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        HealthBar.UpdatePlayerHp();
        //FindObjectOfType<PlayerHealthBar>().UpdatePlayerHp();
        if (currentHealth <= 0)
        {

            Die();
            StartCoroutine(DeathTitle());
        }
    }

    public void Die()
    {
        anim.SetTrigger("doDie");
        deathText.DOText("당신은 "+ "<color=red>" + "몬스터"+ "</color>" + "에게 죽었습니다.", 3, true, ScrambleMode.None, null);

        EndingImage.rectTransform.gameObject.SetActive(true);

        EndDeath();
    }
    IEnumerator DeathTitle()
    {
        KilledImage.rectTransform.gameObject.SetActive(true);
        kill.text = "Player";
        death.text = "Boss";       
        yield return new WaitForSeconds(10f);
                
        KilledImage.DOFade(0f, 1f).OnComplete(() =>
        {
            KilledImage.rectTransform.gameObject.SetActive(false);
        });

    }
    void EndDeath()
    {
        Destroy(gameObject);
    }
}
