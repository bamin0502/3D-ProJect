using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public EnemyHealth enemyHealth;
    public Image healthBar;

    // Update is called once per frame
    void Update()
    {
        healthBar.fillAmount = (float)enemyHealth.currentHealth / enemyHealth.maxHealth;
        transform.LookAt(Camera.main.transform);
    }
}
