using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using MNF;
using TMPro;
using Random = UnityEngine.Random;

public enum Lobby_State //플레이어 상태 0:방장 1:준비됨
{
    IsAdmin,
    IsReady,
}

public class LobbyScene : MonoBehaviour
{
    public static LobbyScene Instance;
    
    [Header("로그인 패널")]
    public TextMeshProUGUI loginAlertText;
    public TMP_InputField inputUserID;
    public GameObject loginPanel;
    public Button loginButton;
    
    [Header("로비 패널")]
    public TextMeshProUGUI lobbyButtonText;
    public TextMeshProUGUI lobbyAlertText;
    public Button lobbyButton;

    [Header("그외")]
    public GameObject playerPrefab;
    public Transform[] positions;

    private readonly Regex _regex = new ("^[a-zA-Z0-9가-힣ㄱ-ㅎㅏ-ㅣ]*$");
    private readonly List<GameObject> _characters = new();
    private const int MaxUserAmount = 5;
    private string _userId;
    private bool _isAdmin;
    private bool _isReady;
    
    
	private void Start()
    {
        //로컬에서 테스트
        
        NetGameManager.instance.ConnectServer("127.0.0.1", 3650, true);
        lobbyButton.onClick.AddListener(OnClick_LobbyButton);
        loginButton.onClick.AddListener(OnClick_Login);
        Instance = this;
        //NetGameManager.instance.ConnectServer("3.34.116.91", 3650); 
        //NetGameManager.instance.ConnectServer("192.168.0.43", 3650, true);
    }

    private void OnClick_Login()
	{
        //로그인 버튼 클릭시
        string userID = inputUserID.text;
        if (userID.Length is < 1 or > 10)
        {
            loginAlertText.text = "아이디의 길이를 1자 이상, 10자 이하로 맞춰주세요";
            return;
        }
        if (!_regex.IsMatch(userID))
        {
            loginAlertText.text = "아이디에 특수문자는 사용할 수 없습니다.";
            return;
        }
        
        NetGameManager.instance.UserLogin(userID, 1);
	}
    private void OnClick_LobbyButton()
	{
        //준비, 시작버튼 클릭시
        RoomSession roomSession = NetGameManager.instance.m_roomSession;
        
        UserSession userSession =
            NetGameManager.instance.GetRoomUserSession(NetGameManager.instance.m_userHandle.m_szUserID);
        
        
        if (userSession.m_nUserData[(int)Lobby_State.IsAdmin] == 1)
        {
            userSession.m_nUserData[(int)Lobby_State.IsReady] = 1;
            NetGameManager.instance.RoomUserDataUpdate(userSession);
            
            for (int i = 0; i < roomSession.m_userList.Count; i++)
            {
                if (roomSession.m_userList[i].m_nUserData[(int)Lobby_State.IsReady] == 0)
                {
                    //한명이라도 준비를 안했으면
                    lobbyAlertText.text = "준비 되지 않은 사람이 있습니다.";
                    return;
                }
            }

            //TODO: 게임 시작
        }
        else
        {
            //TODO: 이미 준비 중이면 준비 취소
            
            userSession.m_nUserData[(int)Lobby_State.IsReady] = 1;
            NetGameManager.instance.RoomUserDataUpdate(userSession);
            _isReady = true;
            PlayerReady();
        }
	}
    public void UserLoginResult(ushort usResult)
	{
        //로그인 결과
        if (usResult == 0) loginPanel.SetActive(false);
        else if (usResult == 125) loginAlertText.text = "이미 존재하는 아이디입니다.";
        else loginAlertText.text = "로그인에 실패했습니다." + usResult;
    }
    private bool CanEnterRoom(string userID)
    {
        RoomSession roomSession = NetGameManager.instance.m_roomSession;
        int userCount = roomSession.m_userList.Count;
        GameObject toDestroy = GameObject.Find(userID);
        
        if (toDestroy != null)
        {
            NetGameManager.instance.RoomUserForcedOut(userID);
            return false;
        }

        if (userCount > MaxUserAmount)
        {
            NetGameManager.instance.RoomUserForcedOut(userID);
            return false;
        }

        return true;
    }
    public void RoomEnter()
	{
        // 새로 들어왔을때
        
        RoomSession roomSession = NetGameManager.instance.m_roomSession;
        int userCount = roomSession.m_userList.Count;
        UserSession userSession = NetGameManager.instance.GetRoomUserSession(NetGameManager.instance.m_userHandle.m_szUserID);
        lobbyAlertText.text = "알림 : " + userSession.m_szUserID + " 님이 입장하셨습니다.";

        if (!CanEnterRoom(userSession.m_szUserID))
        {
            loginPanel.SetActive(true);
            loginAlertText.text = "로그인에 실패했습니다.";
            return;
        }
        
        if (userCount == 1) //해당 로비에 유저가 본인 뿐이면 방의 방장으로 설정
        {
            _isAdmin = true;
            lobbyButtonText.text = "게임 시작";
            userSession.m_nUserData[(int)Lobby_State.IsAdmin] = 1;
            userSession.m_nUserData[(int)Lobby_State.IsReady] = 1;
            NetGameManager.instance.RoomUserDataUpdate(userSession);
        }
        else
        {
            lobbyButtonText.text = "준비";
            userSession.m_nUserData[(int)Lobby_State.IsAdmin] = 0;
            userSession.m_nUserData[(int)Lobby_State.IsReady] = 0;
            NetGameManager.instance.RoomUserDataUpdate(userSession);
        }
        
		for(int i = 0; i < userCount; i++)
		{
            RoomOneUserAdd(roomSession.m_userList[i]);
		}
	}
    public void RoomUserAdd(UserSession user)
	{
        //기존 유저들에게 새로운 유저 들어옴 알림
        //RoomUpdate도 실행됨

        if (!CanEnterRoom(user.m_szUserID)) return;
        
        lobbyAlertText.text = "알림 : " + user.m_szUserID + " 님이 입장하셨습니다.";
        RoomOneUserAdd(user);
	}

    public void RoomUserDel(UserSession user)
	{
        //유저 삭제 및 기존 유저 재정렬
        
        GameObject toDestroy = GameObject.Find(user.m_szUserID);
        
        bool isAdmin = user.m_nUserData[(int)Lobby_State.IsAdmin] == 1;
        
        if (toDestroy != null)
        {
            lobbyAlertText.text = "알림 : " + user.m_szUserID + " 님이 퇴장하셨습니다.";
            int index = _characters.IndexOf(toDestroy);
            if (index < 0) return;

            Destroy(toDestroy);
            _characters.RemoveAt(index);
            
            for (int i = index; i < _characters.Count; i++)
            {
                if (_characters[i] != null)
                {
                    _characters[i].transform.position = positions[i].position;
                }
            }

            if (isAdmin && _characters.Count > 0)
            {
                int randomIndex = Random.Range(0, _characters.Count);
                GameObject selectedObject = _characters[randomIndex];
                UserSession userSession = NetGameManager.instance.GetRoomUserSession(selectedObject.gameObject.name);
                userSession.m_nUserData[(int)Lobby_State.IsAdmin] = 1;
                userSession.m_nUserData[(int)Lobby_State.IsReady] = 1;
                NetGameManager.instance.RoomUserDataUpdate(userSession);
                selectedObject.GetComponent<Lobby_Player>().ChangeIcon(true, true);
            }
        }
	}

    void RoomOneUserAdd(UserSession user)
	{
        //유저 추가
        
        if (_characters.Count > MaxUserAmount) return;
        if (!CanEnterRoom(user.m_szUserID)) return;

        GameObject newCharacter = Instantiate(playerPrefab);
        
        for (int i = 0; i < MaxUserAmount; i++)
        {
            if (_characters.Count <= i || _characters[i] == null)
            {
                bool isAdmin = user.m_nUserData[(int)Lobby_State.IsAdmin] == 1;
                bool isReady = user.m_nUserData[(int)Lobby_State.IsReady] == 1;
                
                newCharacter.transform.position = positions[i].position;
                newCharacter.transform.rotation = Quaternion.Euler(0,180,0);
                newCharacter.TryGetComponent(out Lobby_Player player);
                player.Init(user);
                player.ChangeIcon(isAdmin, isReady);

                if (_characters.Count <= i)
                {
                    _characters.Add(newCharacter);
                }
                else
                {
                    _characters[i] = newCharacter;
                }

                break;
            }
        }
	}

    public void RoomBroadcast(string szData)
	{
        //모든 유저에게 정보 전달
        LitJson.JsonData jData = LitJson.JsonMapper.ToObject(szData);
        string userID = jData["USER"].ToString();
        int    dataID = Convert.ToInt32(jData["DATA"].ToString());


        switch (dataID)
        {
            case 1:
                InvokeRepeating("UserMove", 0, 0.1f);
                break;
            case 2:
                bool isAdmin = bool.Parse(jData["ISADMIN"].ToString());
                bool isReady = bool.Parse(jData["ISREADY"].ToString());

                GameObject toUpdate = GameObject.Find(userID);

                if (toUpdate != null)
                {
                    toUpdate.TryGetComponent(out Lobby_Player player);
                    player.ChangeIcon(isAdmin, isReady);
                }
                
                break;
        }
    }
    
    private void PlayerReady()
    {
        UserSession userSession = NetGameManager.instance.GetRoomUserSession(
            NetGameManager.instance.m_userHandle.m_szUserID);
        
        var data = new LOBBY_STATE
        {
            USER = userSession.m_szUserID,
            DATA = 2,
            ISADMIN = _isAdmin,
            ISREADY = _isReady
        };

        string sendData = LitJson.JsonMapper.ToJson(data);
        NetGameManager.instance.RoomBroadcast(sendData);
    }

    public void OnConnectFail()
    {
        loginAlertText.text = "서버와의 연결에 실패했습니다.";
    }

    public void RoomUpdate()
    {
        //룸 정보 업데이트 (새로운 유저 들어왔을 때 자동 실행됨)
        // RoomSession roomSession = NetGameManager.instance.m_roomSession;
        // for (int i = 0; i < roomSession.m_userList.Count; i++)
        // {
        //     if (roomSession.m_userList[i].m_szUserID !=
        //         NetGameManager.instance.m_userHandle.m_szUserID)
        //     {
        //         GameObject playerObj = GameObject.Find(roomSession.m_userList[i].m_szUserID);
        //         if (playerObj)
        //         {
        //             playerObj.transform.position = roomSession.m_userList[i].m_userTransform[0].GetVector3();
        //         }
        //     }
        // }
    }

    public void RoomUserDataUpdate(UserSession user)
    {
        //NetGameManager에서 RoomUserDataUpdate사용하면 호출됨

        // RoomSession roomSession = NetGameManager.instance.m_roomSession;
        // for (int i = 0; i < roomSession.m_userList.Count; i++)
        // {
        //     if (roomSession.m_userList[i].m_szUserID == user.m_szUserID)
        //     {
        //         GameObject playerObj = GameObject.Find(roomSession.m_userList[i].m_szUserID);
        //         if (playerObj)
        //         {
        //             Destroy(playerObj, 0);
        //         }
        //
        //         RoomOneUserAdd(user);
        //         return;
        //     }
        // }
    }
    
    #region 해당씬에서 안쓰는 거
    
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

    #endregion
}
