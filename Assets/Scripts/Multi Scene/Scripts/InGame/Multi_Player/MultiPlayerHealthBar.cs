using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MultiPlayerHealthBar : MonoBehaviour
{
    public Image healthBar;
    public TextMeshProUGUI healthText;
    private MultiPlayerHealth _playerHealth;
    private string _playerName;
    public string playerName = "";
    public TextMeshProUGUI nameText;
    public void CreateUiStatus(string _playerName)
    {
        MultiScene.Instance._players.TryGetValue(_playerName, out var playerPrefab);
        if (playerPrefab != null)
        {
            MultiPlayerHealthBar _playerHealthBar = GameObject.Find("HpBase").GetComponent<MultiPlayerHealthBar>();
            _playerHealthBar._playerName = _playerName;
            _playerHealth = playerPrefab.GetComponent<MultiPlayerHealth>();

            if (_playerHealth != null)
            {
                Debug.Log(playerPrefab.name + " 체력바 생성");
                healthText.text = _playerHealth.CurrentHealth + "/" + _playerHealth.MaxHealth;
                UpdatePlayerHp();
            }
            else
            {
                Debug.LogError(playerPrefab.name + "체력바 생성에 실패했습니다.");
            }
        }
    }
    public void UpdatePlayerHp()
    {
        healthBar.fillAmount = (float)_playerHealth.CurrentHealth / _playerHealth.MaxHealth;
        healthText.text = _playerHealth.CurrentHealth + "/" + _playerHealth.MaxHealth;
    }
    
}
