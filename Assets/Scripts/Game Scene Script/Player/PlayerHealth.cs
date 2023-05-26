using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public PlayerHealthBar HealthBar;
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        HealthBar.UpdatePlayerHp();
        UImanager.inst.UpdatePlayerHp();

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
