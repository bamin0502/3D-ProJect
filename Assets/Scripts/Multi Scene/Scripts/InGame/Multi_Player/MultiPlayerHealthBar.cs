using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MultiPlayerHealthBar : MonoBehaviour
{
    public Image healthBar;
    public TextMeshProUGUI healthText;
    private MultiPlayerHealth _playerHealth;
    private string _playerName;
    private GameObject player;
    public string playerName = "";
    public void CreateUiStatus(string _playerName)
    {
        MultiScene.Instance._players.TryGetValue(_playerName, out player);
        if (player != null)
        {
            Debug.Log("플레이어를 찾지 못했습니다...");
            _playerHealth = player.GetComponent<MultiPlayerHealth>();
            if (_playerHealth != null)
            {
                Debug.Log(player.name + "체력바 생성");
            }
            else
            {
                Debug.LogError(player.name + "체력바 생성에 실패했습니다.");
            }
        }

        healthText.text = _playerHealth.CurrentHealth + "/" + _playerHealth.MaxHealth;
        UpdatePlayerHp();
    }
    public void UpdatePlayerHp()
    {
        healthBar.fillAmount = (float)_playerHealth.CurrentHealth / _playerHealth.MaxHealth;
        healthText.text = _playerHealth.CurrentHealth + "/" + _playerHealth.MaxHealth;
    }
}
