using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MultiPlayerHealthBar : MonoBehaviour
{
    public Image healthBar;
    public TextMeshProUGUI healthText;
    private MultiPlayerHealth _playerHealth;
    private GameObject player;
    public void CreateUiStatus()
    {
        MultiScene.Instance._players.TryGetValue(MultiScene.Instance.currentUser, out var player);
        _playerHealth = player.GetComponent<MultiPlayerHealth>();
        if (_playerHealth != null)
        {
            Debug.Log(player.name+"체력바 생성");
        }
        else
        {
            Debug.LogError(player.name+"체력바 생성에 실패했습니다.");
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
