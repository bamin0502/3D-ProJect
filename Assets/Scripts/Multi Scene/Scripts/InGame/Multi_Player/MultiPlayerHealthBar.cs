using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MultiPlayerHealthBar : MonoBehaviour
{

    public Image healthBar;
    public TMP_Text healthText;
    
    public void UpdatePlayerHp()
    {
        float currentHealth = MultiPlayerHealth.CurrentHealth;
        float maxHealth = MultiPlayerHealth.MaxHealth;

        // 최소값과 최대값을 제한하여 체력 값을 계산
        float displayedHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        displayedHealth = Mathf.Max(displayedHealth, 0f);

        //음수 값을 0으로 제한=이래야만 -20같은 경우가 나오지않는다.
        if (displayedHealth <= 0f)
        {
            healthBar.fillAmount = 0 / maxHealth;

        }
        else if(displayedHealth>0f)
        {
            // 체력 바 업데이트
            healthBar.fillAmount = displayedHealth / maxHealth;
        }
       



    }    
}
