using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class MultiTeamstatus : MonoBehaviour
{
    public string playerName = "";
    public TextMeshProUGUI nameText;
    public GridLayoutGroup gridLayoutGroup;
    public Image playerHpImage;
    public GameObject statusbar;
    

    public void CreateTeamStatus(string PlayerName)
    {
        GameObject teamStatusObject = Instantiate(statusbar, gridLayoutGroup.transform);
        MultiTeamstatus teamStatus = teamStatusObject.GetComponent<MultiTeamstatus>();
        
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
    public void Awake()
    {
        gridLayoutGroup = GameObject.Find("Team Status Image").GetComponent<GridLayoutGroup>();
    }
}
