using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MultiUiManager : MonoBehaviour
{
    private TMP_Text playerHp;
    public Image playerHpImage;
    public void Update()
    {
        UpdatePlayerHp();
    }
    public void UpdatePlayerHp()
    {
        playerHpImage.fillAmount = (float)MultiPlayerHealth.CurrentHealth / MultiPlayerHealth.MaxHealth;
        playerHp.text = MultiPlayerHealth.CurrentHealth + "/" + MultiPlayerHealth.MaxHealth;

        if (MultiPlayerHealth.CurrentHealth <=0)
        {
            playerHp.text=0 +"/"+ MultiPlayerHealth.MaxHealth;
        }
    }
}
