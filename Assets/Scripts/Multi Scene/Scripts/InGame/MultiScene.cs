using System;
using System.Collections;
using System.Collections.Generic;
using MNF;
using UnityEngine;

public class MultiScene : MonoBehaviour
{
    public Transform[] positions; //유저 찍어낼 위치
    public GameObject playerPrefab; //찍어낼 유저 프리팹
    private string _currentUser = "";
    private void Start()
    {
        RoomSession roomSession = NetGameManager.instance.m_roomSession;
        _currentUser = NetGameManager.instance.m_userHandle.m_szUserID;
        
        for (int i = 0; i < roomSession.m_userList.Count; i++)
        {
            //TODO: 여기서 유저 찍어냄
            //TODO : _currentUser한테 클라이언트의 카메라 붙임
        }
    }

    public void OnClickTest()
    {
        //테스트 용
        RoomSession roomSession = NetGameManager.instance.m_roomSession;
        string user = NetGameManager.instance.m_userHandle.m_szUserID;
        
        Debug.Log($"유저 수 : {roomSession.m_userList.Count} 현재 유저 : {user}");
    }
}
