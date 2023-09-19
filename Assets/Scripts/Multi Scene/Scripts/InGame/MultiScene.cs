using System;
using System.Collections;
using System.Collections.Generic;
using mino;
using MNF;
using UnityEngine;

public class MultiScene : MonoBehaviour
{
    public static MultiScene Instance;
    
    private Dictionary<string, GameObject> _players = new ();

    public CameraController playerCamera;
    public Transform[] positions; //유저 찍어낼 위치
    public GameObject playerPrefab; //찍어낼 유저 프리팹
    private string _currentUser = "";
    private int currentState = -99;

    private GameObject currnetUser;
    private void Start()
    {
        Instance = this;
        RoomSession roomSession = NetGameManager.instance.m_roomSession;
        _currentUser = NetGameManager.instance.m_userHandle.m_szUserID;
        
        for (int i = 0; i < roomSession.m_userList.Count; i++)
        {
            GameObject newPlayer = Instantiate(playerPrefab);
            string newPlayerName = roomSession.m_userList[i].m_szUserID;
            newPlayer.name = newPlayerName;
            newPlayer.transform.position = positions[i].position;
            
            Debug.Log($"{newPlayerName} {newPlayer} {i}");

            _players.Add(newPlayerName, newPlayer);
            
            newPlayer.TryGetComponent<MultiPlayerMovement>(out var multiPlayer);
            
            if (newPlayerName.Equals(_currentUser))
            {
                //만약 현재 유저일경우
                playerCamera.player = newPlayer.transform;
                multiPlayer._camera = playerCamera.mainCamera;
                currnetUser = newPlayer;
            }
        }
    }

    public void BroadCastingAnimation(int animationNumber)
    {
        if (currentState == animationNumber)
            return;

        currentState = animationNumber;
        
        UserSession userSession = NetGameManager.instance.GetRoomUserSession(
            NetGameManager.instance.m_userHandle.m_szUserID);

        var data = new PLAYER_ANIMATION
        {
            USER = userSession.m_szUserID,
            DATA = 1,
            ANI_NUM = animationNumber,
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
                int aniNum = Convert.ToInt32(jData["ANI_NUM"].ToString());
                
                _players.TryGetValue(userID, out var user);
                user.TryGetComponent<MultiPlayerMovement>(out var movement);
                movement.ChangedState((PlayerState)aniNum);
                break;
        }
    }


    private int testCount = 0;
    public void OnClickTest()
    {
        
        //테스트 용
        if (testCount == 0)
        {
            BroadCastingAnimation(0);
        }

        else if (testCount == 1)
        {
            BroadCastingAnimation(1);
        }

        else if (testCount == 2)
        {
            BroadCastingAnimation(8);
        }
        else if (testCount == 3)
        {
            BroadCastingAnimation(4);
            testCount = 0;
        }

        testCount += 1;
        
        
        RoomSession roomSession = NetGameManager.instance.m_roomSession;
        string user = NetGameManager.instance.m_userHandle.m_szUserID;
        
        Debug.Log($"유저 수 : {roomSession.m_userList.Count} 현재 유저 : {user}");
    }
}
