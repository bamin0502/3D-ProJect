using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Data;

public class UiManager : MonoBehaviour
{
    [SerializeField] private TMP_Text playerHp;
    [SerializeField] private Image hpBar;
    
    public TMP_Text playerLevel;
    public TMP_Text playerExp;

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

    public void UpdatePlayerMp(PlayerStat playerStat)
    {
        //mp아직 없음
    }


}
