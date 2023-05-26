using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using Newtonsoft.Json;

public class PlayerHealth : MonoBehaviour
{
    public static int maxHealth;
    public static int currentHealth;
    public PlayerHealthBar HealthBar;
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        string json = "{\"PlayerHealth\": 10, \"Health\": 10}";
        PlayerStat playerStat = JsonConvert.DeserializeObject<PlayerStat>(json);
        maxHealth = (int)playerStat.PlayerHealth;
        currentHealth = (int)playerStat.Health;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        HealthBar.UpdatePlayerHp();        

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        anim.SetTrigger("doDie");
    }

    void EndDeath()
    {
        Destroy(gameObject);
    }
}
