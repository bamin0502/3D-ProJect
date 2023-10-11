using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MultiPlayerHealthBar : MonoBehaviour
{

    public static Image healthBar;
    public static TextMeshProUGUI healthText;
    private MultiPlayerHealth _playerHealth;
    public string myPlayerName = "";
    public void Awake()
    {
        // myPlayerName 초기화
        myPlayerName = "";
        // 플레이어 GameObject를 찾기
        GameObject player = GameObject.Find(myPlayerName);

        if (player != null)
        {
            _playerHealth = player.GetComponent<MultiPlayerHealth>();

            if (_playerHealth != null)
            {
                Debug.Log(myPlayerName + "의 체력바 생성");
            }
            else
            {
                Debug.LogError(myPlayerName + "의 체력바 생성에 실패했습니다.");
            }
        }
        else
        {
            Debug.LogError("플레이어를 찾을 수 없습니다: " + myPlayerName);
        }

        healthText.text = MultiPlayerHealth.CurrentHealth + "/" + MultiPlayerHealth.MaxHealth;
        healthBar = GameObject.FindGameObjectsWithTag("HpBar")[0].GetComponent<Image>();
        healthText = GameObject.FindGameObjectsWithTag("HpText")[0].GetComponent<TextMeshProUGUI>();
        // 플레이어 GameObject를 찾기
        player = GameObject.Find(myPlayerName);

        if (player != null)
        {
            _playerHealth = player.GetComponent<MultiPlayerHealth>();

            if (_playerHealth != null)
            {
                Debug.Log(myPlayerName + "의 체력바 생성");
            }
            else
            {
                Debug.LogError(myPlayerName + "의 체력바 생성에 실패했습니다.");
            }
        }
        else
        {
            Debug.LogError("플레이어를 찾을 수 없습니다: " + myPlayerName);
        }

        healthText.text = MultiPlayerHealth.CurrentHealth + "/" + MultiPlayerHealth.MaxHealth;
        healthBar = GameObject.FindGameObjectsWithTag("HpBar")[0].GetComponent<Image>();
        healthText = GameObject.FindGameObjectsWithTag("HpText")[0].GetComponent<TextMeshProUGUI>();
    }

    public static void UpdatePlayerHp()
    {
        // float currentHealth = MultiPlayerHealth.CurrentHealth;
        // float maxHealth = MultiPlayerHealth.MaxHealth;
        //
        // // 최소값과 최대값을 제한하여 체력 값을 계산
        // float displayedHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        // displayedHealth = Mathf.Max(displayedHealth, 0f);
        //
        // //음수 값을 0으로 제한=이래야만 -20같은 경우가 나오지않는다.
        // if (displayedHealth <= 0f)
        // {
        //     healthBar.fillAmount = 0 / maxHealth;
        // }
        // else if(displayedHealth>0f)
        // {
        //     healthBar.fillAmount = displayedHealth / maxHealth;
        // }
        healthText.text = MultiPlayerHealth.CurrentHealth + "/" + MultiPlayerHealth.MaxHealth;
        healthBar.fillAmount = (float)MultiPlayerHealth.CurrentHealth / MultiPlayerHealth.MaxHealth;
        if (MultiPlayerHealth.CurrentHealth <=0)
        {
            healthText.text=0 +"/"+ MultiPlayerHealth.MaxHealth;
        }    



    }    
}
