using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using MNF;

public class SceneSample : MonoBehaviour
{
    public static SceneSample s;

    public GameObject loginPanel;
    public GameObject gameStartBtn;
    public GameObject[] playerPrefab;
    public InputField inputUserID;
	void Start()
    {
        s = this;
        //NetGameManager.instance.ConnectServer("3.34.116.91", 3650); 
        //NetGameManager.instance.ConnectServer("192.168.0.43", 3650, true);
        NetGameManager.instance.ConnectServer("127.0.0.1", 3650, true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            RoomSession roomSession = NetGameManager.instance.m_roomSession;
            roomSession.m_nRoomData[0] = 11;
            NetGameManager.instance.RoomDataUpdate(roomSession);
        }
     }

    public void OnClick_Login()
	{
        string userID = inputUserID.text;
        if (userID.Length < 1)
            return;

        NetGameManager.instance.UserLogin(userID, 1);
	}

    public void OnClick_Start()
	{
        var data = new GAME_START
        { 
            USER = NetGameManager.instance.m_userHandle.m_szUserID, 
            DATA = 1
        };

        string sendData = LitJson.JsonMapper.ToJson(data);
        NetGameManager.instance.RoomBroadcast(sendData);
	}

    public void OnClick_TankChange ()
    {
        UserSession userSession = NetGameManager.instance.GetRoomUserSession(
            NetGameManager.instance.m_userHandle.m_szUserID);

        userSession.m_nUserData[0] = (byte)UnityEngine.Random.Range(0, 4);
        NetGameManager.instance.RoomUserDataUpdate(userSession);
    }

    void UserMove()
	{
        UserSession userSession = NetGameManager.instance.GetRoomUserSession(
            NetGameManager.instance.m_userHandle.m_szUserID);

        NetGameManager.instance.RoomUserMove(userSession);

        Debug.Log("UserMove : ");
	}

    //발사했을때의 샘플코드
    private void TankFire()
    {
        var data = new TANK_FIRE
        { 
            USER = "User10", 
            DATA = 2,
            Power = "10",
            Position = "1,1,1"
        };

        string sendData = LitJson.JsonMapper.ToJson(data);
        NetGameManager.instance.RoomBroadcast(sendData);
    }

    public void UserLoginResult(ushort usResult)
	{
        if (usResult == 0)
        {
            loginPanel.SetActive(false);
        }

        Debug.Log("UserLoginResult : " + usResult.ToString());
    }

    public void RoomEnter()
	{
        RoomSession roomSession = NetGameManager.instance.m_roomSession;
		for(int i = 0; i < roomSession.m_userList.Count; i++)
		{
            RoomOneUserAdd(roomSession.m_userList[i]);
		}
	}

    public void RoomUserAdd(UserSession user)
	{
        RoomOneUserAdd(user);
	}

    public void RoomUserDel(UserSession user)
	{
        GameObject playerObj = GameObject.Find(user.m_szUserID);
        Destroy(playerObj, 0);        
	}

    void RoomOneUserAdd(UserSession user)
	{
        Vector3 pos = user.m_userTransform[0].GetVector3();

        GameObject playerObj = Instantiate(playerPrefab[user.m_nUserData[0]], pos, transform.rotation);
        playerObj.GetComponent<Player>().Init(user);
	}

    public void RoomBroadcast(string szData)
	{
        LitJson.JsonData jData = LitJson.JsonMapper.ToObject(szData);
        string userID = jData["USER"].ToString();
        int    dataID = Convert.ToInt32(jData["DATA"].ToString());

        Debug.Log("RoomBroadcast : " + userID + " , " + dataID.ToString() );
        if (dataID == 1)//게임시작
		{
            gameStartBtn.SetActive(false);
            InvokeRepeating("UserMove", 0, 0.05f);
		}
        else if (dataID == 2)//총알발사
		{
            string power = jData["Power"].ToString();
            string pos = jData["Position"].ToString();
		}

    }
    public void RoomUserDataUpdate(UserSession user)
    {
        RoomSession roomSession = NetGameManager.instance.m_roomSession;
		for(int i = 0; i < roomSession.m_userList.Count; i++)
		{
            if (roomSession.m_userList[i].m_szUserID == user.m_szUserID)
            {
                GameObject playerObj = GameObject.Find(roomSession.m_userList[i].m_szUserID);
                if (playerObj)
				{
                    Destroy(playerObj, 0);
				}

                RoomOneUserAdd(user);
                return;
            }
		}
    }
    public void RoomUserMoveDirect(UserSession user)
    {
        RoomSession roomSession = NetGameManager.instance.m_roomSession;
        for (int i = 0; i < roomSession.m_userList.Count; i++)
        {
            if (roomSession.m_userList[i].m_szUserID == user.m_szUserID)
            {
                GameObject playerObj = GameObject.Find(roomSession.m_userList[i].m_szUserID);
                if (playerObj)
                {
                    Destroy(playerObj, 0);
                }

                
                return;
            }
        }
    }
    public void RoomUserItemUpdate(UserSession user)
    {
        RoomSession roomSession = NetGameManager.instance.m_roomSession;
        for (int i = 0; i < roomSession.m_userList.Count; i++)
        {
            if (roomSession.m_userList[i].m_szUserID == user.m_szUserID)
            {
                GameObject playerObj = GameObject.Find(roomSession.m_userList[i].m_szUserID);
                if (playerObj)
                {
                    Destroy(playerObj, 0);
                }

               
                return;
            }
        }
    }
    public void RoomUpdate()
	{
        RoomSession roomSession = NetGameManager.instance.m_roomSession;
		for(int i = 0; i < roomSession.m_userList.Count; i++)
		{
            if (roomSession.m_userList[i].m_szUserID !=
                NetGameManager.instance.m_userHandle.m_szUserID)
            {
                GameObject playerObj = GameObject.Find(roomSession.m_userList[i].m_szUserID);
                if (playerObj)
				{
                    playerObj.transform.position = roomSession.m_userList[i].m_userTransform[0].GetVector3();
				}
            }
		}
	}
}
