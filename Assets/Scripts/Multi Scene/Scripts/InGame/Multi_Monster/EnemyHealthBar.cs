using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum EnemyHealthType
{
    Boss,
    Monster,
}

public class EnemyHealthBar : MonoBehaviour
{
    public EnemyHealth enemyHealth;
    public Image healthBar;
    public TMP_Text healthText;
    private Camera _cam;
    public EnemyHealthType enemyHealthType;
    private void Start()
    {
        _cam = Camera.main;
        if (healthText != null)
        {
            healthText.text = (float)enemyHealth.currentHealth + "/" + enemyHealth.maxHealth;
        }

        if (enemyHealthType==EnemyHealthType.Boss)
        {
            UpdateBossHealth();  
        }
        else if(enemyHealthType==EnemyHealthType.Monster)
        {
            UpdateHealth();
        }
    }
    
    public void UpdateHealth()
    {
        healthBar.fillAmount = (float)enemyHealth.currentHealth / enemyHealth.maxHealth;
    }
    public void UpdateBossHealth()
    {
        healthBar.fillAmount= Mathf.Max(enemyHealth.currentHealth / enemyHealth.maxHealth);
        healthText.text =  Mathf.Max(0, enemyHealth.currentHealth) + "/" + enemyHealth.maxHealth;
        
        if(enemyHealth.currentHealth <= 0)
            healthText.text = "0/0";
    }
    void Update()
    {
        Vector3 lookAtPosition = _cam.transform.position;
        lookAtPosition.x = transform.position.x;
        transform.LookAt(lookAtPosition);
       
       
    }
}
