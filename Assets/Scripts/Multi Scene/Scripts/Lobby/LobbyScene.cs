using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using MNF;
using TMPro;

public enum LobbyUserState
{
    Admin,
    Ready,
    NotReady
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
    public Button lobbyButton;

    [Header("채팅")]
    public TextMeshProUGUI chatPrefab;
    public TMP_InputField inputChat;
    public Transform chatViewParent;
    public RectTransform chatRoot;
    public Transform chatBox;
    
    [Header("그외")]
    public GameObject playerPrefab;
    public Transform[] positions;

    private readonly Regex _regex = new ("^[a-zA-Z0-9가-힣ㄱ-ㅎㅏ-ㅣ]*$");
    private readonly List<GameObject> _characters = new();
    private const int MinUserToStart = 2;
    private const int MaxUserAmount = 5;
    private string _userId;
    
    private void Start()
    {
        //로컬에서 테스트
        //NetGameManager.instance.ConnectServer("127.0.0.1", 3650, true);
        lobbyButton.onClick.AddListener(OnClick_LobbyButton);
        loginButton.onClick.AddListener(OnClick_Login);
        inputChat.onSubmit.AddListener(SendChatting);
        Instance = this;
        NetGameManager.instance.ConnectServer("3.34.116.91", 3650); 
        //NetGameManager.instance.ConnectServer("192.168.0.43", 3650, true);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 localMousePosition = chatRoot.InverseTransformPoint(Input.mousePosition);
            if (!chatRoot.rect.Contains(localMousePosition)) chatBox.gameObject.SetActive(false);
        }
    }
    
    
    private void OnClick_LobbyButton()
	{
        //준비, 시작버튼 클릭시
        RoomSession roomSession = NetGameManager.instance.m_roomSession;
        
        UserSession userSession =
            NetGameManager.instance.GetRoomUserSession(NetGameManager.instance.m_userHandle.m_szUserID);
        
        if (userSession.m_nUserData[0] == (int)LobbyUserState.Admin) //어드민일 경우
        {
            if (roomSession.m_userList.Any(t => t.m_nUserData[0] == (int)LobbyUserState.NotReady))
            {
                string chatText = "<#4FB7FF><b>알림 : 준비 되지 않은 사람이 있습니다.</b></color>";
                AddChatting(chatText);
                return;
            }
            if (roomSession.m_userList.Count < MinUserToStart)
            {
                string chatText = "<#4FB7FF><b>알림 : 시작에 필요한 최소 인원이 부족합니다.</b></color>";
                AddChatting(chatText);
                return;
            }

            GameStart();
        }
        else
        {
            if (userSession.m_nUserData[0] == (int)LobbyUserState.Ready)
            {
                userSession.m_nUserData[0] = (int)LobbyUserState.NotReady;
            }
            else 
            {
                userSession.m_nUserData[0] = (int)LobbyUserState.Ready;
            }

            NetGameManager.instance.RoomUserDataUpdate(userSession);
        }
	}
    public void RoomEnter()
	{
        // 새로 들어왔을때
        
        if (!CanEnterRoom(NetGameManager.instance.m_userHandle.m_szUserID))
        {
            loginPanel.SetActive(true);
            loginAlertText.text = "로그인에 실패했습니다.";
            return;
        }

        RoomSession roomSession = NetGameManager.instance.m_roomSession;
        int userCount = roomSession.m_userList.Count;
        UserSession userSession = NetGameManager.instance.GetRoomUserSession(NetGameManager.instance.m_userHandle.m_szUserID);
        string chatText = $"<#4FB7FF><b>알림 : {userSession.m_szUserID} 님이 입장하셨습니다.</b></color>";
        BroadcastChat(chatText);
        
        if (userCount == 1) //해당 로비에 유저가 본인 뿐이면 방의 방장으로 설정
        {
            lobbyButtonText.text = "게임 시작";
            userSession.m_nUserData[0] = (int)LobbyUserState.Admin;
        }
        else
        {
            lobbyButtonText.text = "준비";
            userSession.m_nUserData[0] = (int)LobbyUserState.NotReady;
        }

        NetGameManager.instance.RoomUserDataUpdate(userSession);
        
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
        RoomOneUserAdd(user);
	}
    void RoomOneUserAdd(UserSession user)
    {
        //유저 추가
        if (!CanEnterRoom(user.m_szUserID)) return;

        GameObject newCharacter = Instantiate(playerPrefab);

        for (int i = 0; i < MaxUserAmount; i++)
        {
            if (_characters.Count <= i || _characters[i] == null)
            {
                newCharacter.transform.position = positions[i].position;
                newCharacter.transform.rotation = Quaternion.Euler(0, 180, 0);
                newCharacter.TryGetComponent(out Lobby_Player player);
                
                player.Init(user);
                player.ChangeIcon(user.m_nUserData[0]);

                if (_characters.Count <= i) _characters.Add(newCharacter);
                else _characters[i] = newCharacter;
                break;
            }
        }
    }
    public void RoomUserDel(UserSession user)
	{
        //유저 삭제 및 기존 유저 재정렬
        
        GameObject toDestroy = _characters.FirstOrDefault(character => character.name == user.m_szUserID);
        
        if (toDestroy != null)
        {
            string chatText = $"<#4FB7FF><b>알림 : {user.m_szUserID} 님이 퇴장하셨습니다.</b></color>";
            AddChatting(chatText);
            
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

            if (user.m_nUserData[0] == (int) LobbyUserState.Admin && NetGameManager.instance.m_roomSession.m_userList.Count > 0)
            {
                UserSession userSession = NetGameManager.instance.m_roomSession.m_userList[0];
                userSession.m_nUserData[0] = (int)LobbyUserState.Admin;
                NetGameManager.instance.RoomUserDataUpdate(userSession);
            }
        }
	}
    

    public void RoomUpdate()
    {
        //룸 정보 업데이트 (새로운 유저 들어왔을 때 자동 실행됨)
    }
    public void RoomUserDataUpdate(UserSession user)
    {
        //NetGameManager에서 RoomUserDataUpdate사용하면 호출됨

        GameObject character = _characters.FirstOrDefault(character => character.name == user.m_szUserID);
        if(character == null) return;
        
        character.TryGetComponent<Lobby_Player>(out var toUpdate);
        toUpdate.ChangeIcon(user.m_nUserData[0]);

        if (user.m_nUserData[0] == (int)LobbyUserState.Admin && user.m_szUserID == _userId)
        {
            lobbyButtonText.text = "게임 시작";
        }
    }
    

    #region 브로드 캐스팅

    private void GameStart()
    {
        UserSession userSession = NetGameManager.instance.GetRoomUserSession(
            NetGameManager.instance.m_userHandle.m_szUserID);

        var data = new GAME_CHAT
        {
            USER = userSession.m_szUserID,
            DATA = 1,
        };

        string sendData = LitJson.JsonMapper.ToJson(data);
        NetGameManager.instance.RoomBroadcast(sendData);
    }

    public void RoomBroadcast(string szData)
    {
        //모든 유저에게 정보 전달

        LitJson.JsonData jData = LitJson.JsonMapper.ToObject(szData);
        string userID = jData["USER"].ToString();
        int dataID = Convert.ToInt32(jData["DATA"].ToString());


        switch (dataID)
        {
            case 1:
                //InvokeRepeating("UserMove", 0, 0.1f);
                LoadingSceneManager.LoadScene("Game Scene");
                break;
            case 3:
                var spawnedText = Instantiate(chatPrefab, chatViewParent.transform, false);
                spawnedText.text = jData["CHAT"].ToString();
                break;
        }
    }
    
    

    #endregion

    #region 로그인

    public void OnConnectFail()
    {
        loginAlertText.text = "서버와의 연결에 실패했습니다.";
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

    private void OnClick_Login()
    {
        //로그인 버튼 클릭시
        _userId = inputUserID.text;
        if (_userId.Length is < 1 or > 10)
        {
            loginAlertText.text = "아이디의 길이를 1자 이상, 10자 이하로 맞춰주세요";
            return;
        }

        if (!_regex.IsMatch(_userId))
        {
            loginAlertText.text = "아이디에 특수문자는 사용할 수 없습니다.";
            return;
        }

        NetGameManager.instance.UserLogin(_userId, 1);
    }

    public void UserLoginResult(ushort usResult)
    {
        //로그인 결과
        if (usResult == 0) loginPanel.SetActive(false);
        else if (usResult == 125) loginAlertText.text = "이미 존재하는 아이디입니다.";
        else loginAlertText.text = "로그인에 실패했습니다." + usResult;
    }
    

    #endregion

    #region 채팅

    private void AddChatting(string text)
    {
        var spawnedText = Instantiate(chatPrefab, chatViewParent.transform, false);
        spawnedText.text = text;
    }

    private void SendChatting(string text)
    {
        if (string.IsNullOrWhiteSpace(inputChat.text)) return;

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            string chatText = $"{_userId} : {text}";
            BroadcastChat(chatText);
            inputChat.text = "";
            inputChat.ActivateInputField();
        }
    }

    private void BroadcastChat(string chat)
    {
        UserSession userSession = NetGameManager.instance.GetRoomUserSession(
            NetGameManager.instance.m_userHandle.m_szUserID);

        var data = new GAME_CHAT
        {
            USER = userSession.m_szUserID,
            DATA = 3,
            CHAT = chat,
        };

        string sendData = LitJson.JsonMapper.ToJson(data);
        NetGameManager.instance.RoomBroadcast(sendData);
    }

    #endregion
    
    #region 해당씬에서 안쓰는 거
    
    public void RoomUserMoveDirect(UserSession user)
    {
    }
    public void RoomUserItemUpdate(UserSession user)
    {
    }

    #endregion
}
