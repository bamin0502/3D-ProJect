using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public EnemyHealth enemyHealth;
    public Image healthBar;

    private Camera _cam;

    private void Start()
    {
        _cam = Camera.main;
    }

    public void UpdateHealth()
    {
        healthBar.fillAmount = (float)enemyHealth.currentHealth / enemyHealth.maxHealth;
    }

    void Update()
    {
        Vector3 lookAtPosition = _cam.transform.position;
        lookAtPosition.x = transform.position.x;
        transform.LookAt(lookAtPosition);
    }
}
