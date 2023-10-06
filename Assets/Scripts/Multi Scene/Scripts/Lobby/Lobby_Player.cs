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
        
        if (state == (int)LobbyUserState.Admin) //어드민이면
        {
            topHeadIconImage.sprite = topHeadIconSprites[0];
            return;
        }

        if ((int)LobbyUserState.Ready == state) //준비 완료
        {
            topHeadIconImage.sprite = topHeadIconSprites[1];
        }
        else
        {
            topHeadIconImage.enabled = false;
        }
    }
}
