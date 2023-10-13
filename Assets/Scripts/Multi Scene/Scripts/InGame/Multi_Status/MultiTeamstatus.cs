using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;


public class MultiTeamstatus : MonoBehaviour
{
    public string playerName = "";
    public TextMeshProUGUI nameText;
    public GridLayoutGroup gridLayoutGroup;
    public static Image playerHpImage;
    public GameObject statusbar;

    private MultiPlayerHealth _playerHealth;
    
    public void Awake()
    {
        gridLayoutGroup = GameObject.Find("Team Status Image").GetComponent<GridLayoutGroup>();

    }

    public void CreateTeamStatus(string PlayerName)
    {   
        GameObject teamStatusObject = Instantiate(statusbar, gridLayoutGroup.transform);
        MultiTeamstatus teamStatus = teamStatusObject.GetComponent<MultiTeamstatus>();
        
        MultiScene.Instance._players.TryGetValue(PlayerName, out var player);
        _playerHealth = player.GetComponent<MultiPlayerHealth>();
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
    }
    
}
