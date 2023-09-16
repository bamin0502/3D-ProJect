using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MNF;
using TMPro;
using UnityEngine.UI;

public class Lobby_Player : MonoBehaviour
{
    public TextMeshProUGUI userID;
    public Image topHeadIconImage; //머리 위에 아이콘
    public Sprite[] topHeadIconSprites; // 0 : 방장, 1 : 준비됨, 2 : 준비 안됨

	public void Init(UserSession user)
	{
        gameObject.name = user.m_szUserID;
        userID.text = user.m_szUserID;
    }

    public void ChangeIcon(bool isAdmin, bool isReady)
    {
        if (isAdmin) //어드민이면
        {
            topHeadIconImage.sprite = topHeadIconSprites[0];
            return;
        }

        topHeadIconImage.sprite = isReady ? topHeadIconSprites[1] : topHeadIconSprites[2]; // 준비 완료이면
    }
}
