using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealthBar : MonoBehaviour
{
    public EnemyHealth enemyHealth;
    public Image healthBar;
    public TMP_Text healthText;
    private Camera _cam;
    private void Start()
    {
        _cam = Camera.main;
       
    }
    public void UpdateHealth()
    {
        healthBar.fillAmount = (float)enemyHealth.currentHealth / enemyHealth.maxHealth;
    }
    public void UpdateBossHealth()
    {
        if (enemyHealth.currentHealth > 0)
        {
            healthBar.fillAmount = (float)enemyHealth.currentHealth / enemyHealth.maxHealth;
            healthText.text = (float)enemyHealth.currentHealth + "/" + enemyHealth.maxHealth;
        }
        else
        {
            healthBar.fillAmount = 0f;
            healthText.text = "0/" + enemyHealth.maxHealth;
        }
    }
    void Update()
    {
        Vector3 lookAtPosition = _cam.transform.position;
        lookAtPosition.x = transform.position.x;
        transform.LookAt(lookAtPosition);
    }
}
