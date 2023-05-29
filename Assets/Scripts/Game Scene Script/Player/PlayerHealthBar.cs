using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    private Camera _cam;
    private void Start()
    {
        _cam = Camera.main;
    }

    void Update()
    {
        Vector3 lookAtPosition = _cam.transform.position;
        lookAtPosition.x = transform.position.x;
        transform.LookAt(lookAtPosition);
        if (Time.timeScale == 0)
        {
            return;
        }
    }
    
    public Image healthBar;

    public void UpdatePlayerHp()
    {
        float currentHealth = PlayerHealth.currentHealth;
        float maxHealth = PlayerHealth.maxHealth;

        // 최소값과 최대값을 제한하여 체력 값을 계산
        float displayedHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        // 체력 바 업데이트
        healthBar.fillAmount = displayedHealth / maxHealth;


    }
    
    
}
