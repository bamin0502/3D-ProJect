using UnityEngine;

using MNF;
using TMPro;
using UnityEngine.UI;

public class Lobby_Player : MonoBehaviour
{
    public TextMeshProUGUI userID;
    public Image topHeadIconImage; //머리 위에 아이콘
    public Sprite[] topHeadIconSprites; // 0 : 방장, 1 : 준비됨

	public void Init(UserSession user)
	{
        gameObject.name = user.m_szUserID;
        userID.text = user.m_szUserID;
    }

    public void ChangeIcon(int state)
    {
        topHeadIconImage.enabled = true;
        
        switch (state)
        {
            //어드민이면
            case (int)LobbyUserState.Admin:
                topHeadIconImage.sprite = topHeadIconSprites[0];
                return;
            //준비 완료
            case (int)LobbyUserState.Ready:
                topHeadIconImage.sprite = topHeadIconSprites[1];
                break;
            default:
                topHeadIconImage.enabled = false;
                break;
        }
    }
}
