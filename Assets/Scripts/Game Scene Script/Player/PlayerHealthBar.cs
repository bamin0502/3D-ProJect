using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthBar : MonoBehaviour
{
    private Camera _cam;
    public Image healthBar;
    public TMP_Text healthText;
    private void Start()
    {
        _cam = Camera.main;
    }

    void Update()
    {
        Vector3 lookAtPosition = _cam.transform.position;
        lookAtPosition.x = transform.position.x;
        transform.LookAt(lookAtPosition);
    }
    


    public void UpdatePlayerHp()
    {
        float currentHealth = PlayerHealth.currentHealth;
        float maxHealth = PlayerHealth.maxHealth;

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
