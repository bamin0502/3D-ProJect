using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public Image healthBar;

    public void UpdatePlayerHp()
    {
        healthBar.fillAmount = (float)playerHealth.currentHealth / playerHealth.maxHealth;
    }
    
    void Update()
    {
        transform.LookAt(Camera.main.transform);
    }
}
