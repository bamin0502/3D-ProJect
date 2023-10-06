using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MultiUiManager : MonoBehaviour
{
    [SerializeField] private TMP_Text playerHp;
    [SerializeField] private Image hpBar;
    public void Update()
    {
        UpdatePlayerHp();
    }
    public void UpdatePlayerHp()
    {
        hpBar.fillAmount = (float)MultiPlayerHealth.CurrentHealth / MultiPlayerHealth.MaxHealth;
        playerHp.text = MultiPlayerHealth.CurrentHealth + "/" + MultiPlayerHealth.MaxHealth;

        if (MultiPlayerHealth.CurrentHealth <=0)
        {
            playerHp.text=0 +"/"+ MultiPlayerHealth.MaxHealth;
        }
    }
}
