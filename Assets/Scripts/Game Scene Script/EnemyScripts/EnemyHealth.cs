using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Data;

public enum EnemyType
{
    Boss,
    Default,

}


public class EnemyHealth : MonoBehaviour
{
    public EnemyHealthBar enemyHealthBar;
    public int maxHealth;
    public int currentHealth;
    public Animator anim;
    public EnemyType enemyType = EnemyType.Default;
    

    // Start is called before the first frame update
    void Start()
    {
        string json = "";

        if (enemyType == EnemyType.Default)
        {
            json = "{\"EnemyHealth\": 100, \"Health\": 100}";
        }

        else if (enemyType == EnemyType.Boss)
        {
            json = "{\"EnemyHealth\": 10, \"Health\": 10}";
        }

        EnemyStat enemyStat1 = JsonConvert.DeserializeObject<EnemyStat>(json);
        maxHealth = (int)enemyStat1.EnemyHealth;
        currentHealth = (int)enemyStat1.Health;
        currentHealth = maxHealth;
        enemyHealthBar = GetComponentInChildren<EnemyHealthBar>();
    }

    public void TakeDamage(int damage)
    {
        if(enemyType == EnemyType.Default)
        {
            currentHealth -= damage;
            enemyHealthBar.UpdateHealth();
        }
        else if(enemyType == EnemyType.Boss)
        {
            currentHealth -= damage;
            enemyHealthBar.UpdateBossHealth();
        }
        
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }


    }

    void Die()
    {
        Debug.Log("Enemy died!");
        bool isEnemy = gameObject.TryGetComponent(out Enemy enemy);
        bool isBoss = gameObject.TryGetComponent(out Boss boss);
        if (isEnemy)
        {
            enemy.isDead = true;
        }
        else if (isBoss) {
            boss.isDead = true;
        }
        anim.SetTrigger("doDie");
    }
    
    void EndDeath()
    {
        Destroy(gameObject);
    }
}
