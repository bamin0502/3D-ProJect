using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiManager : MonoBehaviour
{
    [SerializeField] private TMP_Text playerHp;
    [SerializeField] private Image hpBar;
    public void Update()
    {
        UpdatePlayerHp();
    }
    public void UpdatePlayerHp()
    {
        hpBar.fillAmount = (float)PlayerHealth.currentHealth / PlayerHealth.maxHealth;
        playerHp.text = PlayerHealth.currentHealth + "/" + PlayerHealth.maxHealth;

        if (PlayerHealth.currentHealth <=0)
        {
            playerHp.text=0 +"/"+ PlayerHealth.maxHealth;
        }
    }
}
