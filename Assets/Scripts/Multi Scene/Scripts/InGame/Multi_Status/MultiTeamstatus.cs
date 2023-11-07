using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MultiTeamstatus : MonoBehaviour
{
    public string playerName = "";
    public TextMeshProUGUI nameText;
    [SerializeField]public GridLayoutGroup gridLayoutGroup;
    public Image playerHpImage;
    public GameObject statusbar;
    private MultiPlayerHealth _playerHealth;
    public Gradient gradient;
    
    public void CreateTeamStatus(string PlayerName)
    {   
        GameObject teamStatusObject = Instantiate(statusbar, gridLayoutGroup.transform);
        MultiTeamstatus teamStatus = teamStatusObject.GetComponent<MultiTeamstatus>();
        
        MultiScene.Instance._players.TryGetValue(PlayerName, out var player);
        _playerHealth = player!.GetComponent<MultiPlayerHealth>();
        if (_playerHealth != null)
        {
            Debug.Log(PlayerName+ " 체력바 생성");
        }
        else
        {
            Debug.LogError(PlayerName + "체력바 생성에 실패했습니다.");
        }
        teamStatus.playerName = playerName;
        teamStatus.nameText.text = playerName;
        
        TextMeshProUGUI textComponent = teamStatusObject.GetComponentInChildren<TextMeshProUGUI>();
        Transform playerHpTransform = teamStatusObject.transform.Find("PlayerHp");
        if (playerHpTransform != null)
        {
            playerHpImage = playerHpTransform.GetComponent<Image>(); // playerHpImage를 초기화
        }
        GradientColorKey[] colorKeys = new GradientColorKey[6];
        colorKeys[0].color = Color.red;
        colorKeys[0].time = 0.0f;
        colorKeys[1].color = Color.red;
        colorKeys[1].time = 0.25f;
        colorKeys[2].color = Color.yellow;
        colorKeys[2].time = 0.25f;
        colorKeys[3].color = Color.yellow;
        colorKeys[3].time = 0.75f;
        colorKeys[4].color = Color.green;
        colorKeys[4].time = 0.75f;
        colorKeys[5].color = Color.green;
        colorKeys[5].time = 1f;

        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[6];
        for (int i = 0; i < 6; i++)
        {
            alphaKeys[i].alpha = 1.0f;
            alphaKeys[i].time = colorKeys[i].time;
        }

        gradient = new Gradient();
        gradient.SetKeys(colorKeys, alphaKeys);
    }

    public void DestroyTeamStatus(string PlayerName)
    {
        GameObject[] teamStatusObjects = GameObject.FindGameObjectsWithTag("TeamStatus");
        foreach (GameObject teamStatusObject in teamStatusObjects)
        {
            MultiTeamstatus teamStatus = teamStatusObject.GetComponent<MultiTeamstatus>();
            if (teamStatus.playerName.Equals(PlayerName))
            {
                Destroy(teamStatusObject);
            }
        }
    }
    public void UpdatePlayerHp()
    {
        playerHpImage.fillAmount = (float)_playerHealth.CurrentHealth / _playerHealth.MaxHealth;
        float fillAmount = (float)_playerHealth.CurrentHealth / _playerHealth.MaxHealth;
        playerHpImage.fillAmount = fillAmount;
        playerHpImage.color = gradient.Evaluate(fillAmount);
    }
    
}
