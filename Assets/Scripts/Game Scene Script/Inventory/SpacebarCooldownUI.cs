using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mino;

//델리게이트 선언
public delegate void MyDelegate();
public class SpacebarCooldownUI : MonoBehaviour
{
    
    public UnityEngine.UI.Image fill;
    private float MaxCooldown = 10f;
    private float currentCooldown = 10f;
    public static event MyDelegate UIEvent; 
    
    public void SetMaxCooldown(in float value)
    {
        MaxCooldown = value;
        UpdateFillAmount();
    }
    public void SetCurrentCooldown(in float value)
    {
        currentCooldown = value;
        UpdateFillAmount();
    }
    private void UpdateFillAmount()
    {
        fill.fillAmount = currentCooldown / MaxCooldown;
    }
    // Update is called once per frame
    public void Update()
    {

       
    }
    public void SpaceBarUI()
    {
        SetCurrentCooldown(currentCooldown - Time.deltaTime);

        if (currentCooldown < 0f)
            currentCooldown = MaxCooldown;
    }
}
