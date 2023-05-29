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

    // Start is called before the first frame update
    void Start()
    {
        string json = "{\"PlayerHealth\": 100, \"Health\": 100}";
        PlayerStat playerStat = JsonConvert.DeserializeObject<PlayerStat>(json);
        maxHealth = (int)playerStat.PlayerHealth;
        currentHealth = (int)playerStat.Health;

    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        HealthBar.UpdatePlayerHp();
        FindObjectOfType<PlayerHealthBar>().UpdatePlayerHp();
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        anim.SetTrigger("doDie");
        deathText.DOText("당신은 "+ "<color=red>" + "몬스터"+ "</color>" + "에게 죽었습니다.", 3, true, ScrambleMode.None, null);

        EndingImage.rectTransform.gameObject.SetActive(true);

        EndDeath();
    }

    void EndDeath()
    {
        Destroy(gameObject);
    }
}
