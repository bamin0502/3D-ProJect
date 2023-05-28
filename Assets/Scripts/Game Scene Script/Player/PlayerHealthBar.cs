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
        healthBar.fillAmount = (float)PlayerHealth.currentHealth / PlayerHealth.maxHealth;

    }
    
    
}
