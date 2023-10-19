using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Net;
using UnityEngine.SceneManagement;

using MNF;

public class NetGameManager : KWSingleton<NetGameManager>
{
	public readonly UserHandle m_userHandle = new UserHandle();    // 유저개인정보
	public readonly RoomSession m_roomSession = new RoomSession(); // 방정보

	override public void Awake()
	{
		base.Awake();
	}

	public void ConnectServer(string szIP, int nPort, bool isIntranet = false)
	{
		NetManager.instance.Init(szIP, nPort, isIntranet);	
	}

	//방에서 특정유저정보 가져오기
	public UserSession GetRoomUserSession(string szUserID)
    {
        return m_roomSession.m_userList.FirstOrDefault(t => t.m_szUserID == szUserID);
    }

    //방에서 특정유저 강제퇴장
    public void RoomUserForcedOut(string userID)
    {
        for (int i = 0; i < m_roomSession.m_userList.Count; i++)
        {
            if (m_roomSession.m_userList[i].m_szUserID == userID)
            {
                m_roomSession.m_userList.RemoveAt(i);
                break;
            }
        }
    }

	//본인 방입장
	public void Recv_ROOM_ENTER(BinaryReader br)
	{
		m_roomSession.ReadBin(br);

		Debug.Log("Recv_ROOM_ENTER : " + m_roomSession.m_RoomNo.ToString() );

		LobbyScene.Instance.RoomEnter();
	}

	//다른 유저 방입장
	public void Recv_ROOM_MAN_IN(BinaryReader br)
	{
		UserSession userSession = new UserSession();
		userSession.ReadBin(br);

		m_roomSession.m_userList.Add(userSession);

		Debug.Log("Recv_ROOM_MAN_IN : " + userSession.m_szUserID );

		LobbyScene.Instance.RoomUserAdd(userSession);
	}

	//다른유저 방 퇴장
	public void Recv_ROOM_MAN_OUT(BinaryReader br)
	{
		UserSession userSession = new UserSession();
		userSession.ReadBin(br);

		for(int i = 0; i < m_roomSession.m_userList.Count; i++)
		{
			if (m_roomSession.m_userList[i].m_szUserID == userSession.m_szUserID)
			{
				m_roomSession.m_userList.RemoveAt(i);
                
				break;
			}
		}

        if (SceneManager.GetActiveScene().name == "Lobby Scene")
        {
            LobbyScene.Instance.RoomUserDel(userSession);
        }
        else
        {
            MultiScene.Instance.RoomUserDel(userSession);
        }
		

		Debug.Log("Recv_ROOM_MAN_OUT : " + userSession.m_szUserID );
	}

	//방에서 패킷 주고 받기
	public void Recv_ROOM_BROADCAST(BinaryReader br)
	{
		string szData = "";
		NetString.ReadString(br, ref szData);

		Debug.Log("Recv_ROOM_BROADCAST" + szData);

        if (SceneManager.GetActiveScene().name == "Lobby Scene")
        {
            if (LobbyScene.Instance != null)
            {
                LobbyScene.Instance.RoomBroadcast(szData);
            }
            else
            {
                Debug.LogError("LobbyScene.Instance is null");
            }
        }
        if(SceneManager.GetActiveScene().name=="Game Scene")
        {
            if (MultiScene.Instance != null)
            {
                MultiScene.Instance.RoomBroadcast(szData);
            }
            else
            {
                Debug.LogError("MultiScene.Instance is null");
            }
        }
	}

	//방에서 본인정보 업데이트
	public void Recv_ROOM_USER_DATA_UPDATE(BinaryReader br)
	{
		UserSession userSession = new UserSession();
		userSession.ReadBin(br);

		foreach (var t in m_roomSession.m_userList.Where(t => t.m_szUserID == userSession.m_szUserID))
        {
            t.UserDataUpdate(userSession);
            break;
        }		

		Debug.Log("Recv_ROOM_USER_DATA_UPDATE" + userSession.m_szUserID);

		LobbyScene.Instance.RoomUserDataUpdate(userSession);
	}
    public void Recv_ROOM_USER_MOVE_DIRECT(BinaryReader br)
    {
        UserSession userSession = new UserSession();
        userSession.ReadBin(br);

        foreach (var t in m_roomSession.m_userList.Where(t => t.m_szUserID == userSession.m_szUserID))
        {
            t.UserMoveDirect(userSession);
            break;
        }

        Debug.Log("Recv_ROOM_USER_MOVE_DIRECT" + userSession.m_szUserID);

        LobbyScene.Instance.RoomUserMoveDirect(userSession);
    }
    public void Recv_ROOM_USER_ITEM_UPDATE(BinaryReader br)
    {
        UserSession userSession = new UserSession();
        userSession.ReadBin(br);

        foreach (var t in m_roomSession.m_userList.Where(t => t.m_szUserID == userSession.m_szUserID))
        {
            t.UserDataUpdate(userSession);
            break;
        }

        Debug.Log("Recv_ROOM_USER_ITEM_UPDATE" + userSession.m_szUserID);

        LobbyScene.Instance.RoomUserItemUpdate(userSession);
    }
    public void Recv_ROOM_DATA_UPDATE(BinaryReader br)
    {
        m_roomSession.ReadRoomDataUpdate(br);
        Debug.Log("Recv_ROOM_DATA_UPDATE : " + m_roomSession.m_RoomNo.ToString());
    }
    public void Recv_ROOM_UPDATE(BinaryReader br)
	{
		m_roomSession.ReadBin(br);
		Debug.Log("Recv_ROOM_UPDATE : " + m_roomSession.m_RoomNo.ToString() );

		LobbyScene.Instance.RoomUpdate();
	}

	public void UserLogin(string szID, byte byGroup)
	{
		NetManager.instance.Send_WAIT_LOGIN(szID, byGroup, this.gameObject);	
	}

	public void OnRecvWaitLogin(BinaryReader br)
	{
		ushort usResult = br.ReadUInt16();
		m_userHandle.ReadBin(br);

		LobbyScene.Instance.UserLoginResult(usResult);

        Debug.Log("OnRecvWaitLogin");
    }

	public void RoomBroadcast(string szData)
	{
		NetManager.instance.Send_ROOM_BROADCAST( szData );	
	}
	public void RoomUserDataUpdate(UserSession userSession)
	{
		NetManager.instance.Send_ROOM_USER_DATA_UPDATE( userSession );	
	}
	public void RoomUserMove(UserSession userSession)
	{
		NetManager.instance.Send_ROOM_USER_MOVE( userSession );	
	}
    public void RoomUserMoveDirect(UserSession userSession)
    {
        NetManager.instance.Send_ROOM_USER_MOVE_DIRECT(userSession);
    }
    public void RoomUserItemUpdate(UserSession userSession)
    {
        NetManager.instance.Send_ROOM_USER_ITEM_UPDATE(userSession);
    }
    public void RoomDataUpdate(RoomSession roomSession)
    {
        NetManager.instance.Send_ROOM_DATA_UPDATE(roomSession);
    }
    
}
