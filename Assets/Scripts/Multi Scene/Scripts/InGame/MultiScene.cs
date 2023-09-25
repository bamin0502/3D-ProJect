using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Data;
using mino;
using MNF;
using Unity.VisualScripting;
using UnityEngine;

public class MultiScene : MonoBehaviour
{
    public static MultiScene Instance;
    
    private Dictionary<string, GameObject> _players = new ();

    public CinemachineFreeLook cineCam;
    public CameraController playerCamera;
    public Transform[] positions; //유저 찍어낼 위치
    public GameObject playerPrefab; //찍어낼 유저 프리팹
    private string _currentUser = "";
    private int currentState = -99;

    private GameObject currentUser;
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
                currentUser = newPlayer;
                cineCam.Follow = newPlayer.transform;
                cineCam.LookAt = newPlayer.transform;
                cineCam.GetRig(1).LookAt = newPlayer.transform;
            }
        }
    }

    public void BroadCastingAnimation(int animationNumber)
    {
        //if (currentState == animationNumber) return;
        // currentState = animationNumber;
        
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
    
    public void BroadCastingMovement(Vector3 destination)
    {
        UserSession userSession = NetGameManager.instance.GetRoomUserSession(
            NetGameManager.instance.m_userHandle.m_szUserID);

        var data = new PLAYER_MOVE
        {
            USER = userSession.m_szUserID,
            DATA = 2,
            POSITION = VectorToString(destination),
        };

        string sendData = LitJson.JsonMapper.ToJson(data);
        NetGameManager.instance.RoomBroadcast(sendData);
    }

    private string VectorToString(Vector3 position)
    {
        string result = $"{position.x},{position.y},{position.z}";
        return result;
    }
    
    private Vector3 StringToVector(string position)
    {
        string[] posString = position.Split(",");
        Vector3 result = new Vector3(float.Parse(posString[0]), float.Parse(posString[1]), float.Parse(posString[2]));
        return result;
    }
    

    public void RoomBroadcast(string szData)
    {
        //모든 유저에게 정보 전달
        LitJson.JsonData jData = LitJson.JsonMapper.ToObject(szData);
        string userID = jData["USER"].ToString();
        int dataID = Convert.ToInt32(jData["DATA"].ToString());

        if(_currentUser.Equals(userID)) return;
        
        switch (dataID)
        {
            case 1:
                int aniNum = Convert.ToInt32(jData["ANI_NUM"].ToString());
                _players.TryGetValue(userID, out var user);
                if (user != null) user.GetComponent<MultiPlayerMovement>().ChangedState((PlayerState)aniNum);
                break;
            
            case 2:
                _players.TryGetValue(userID, out var userMove);
                userMove.TryGetComponent<MultiPlayerMovement>(out var userMove2);
                userMove2._navAgent.SetDestination(StringToVector(jData["POSITION"].ToString()));
                break;
            case 3:
                _players.TryGetValue(userID, out var userItem);
                userItem.TryGetComponent<ItemPickup>(out var userItem2);
                userItem.GetComponent<ItemPickup>().item = userItem2.item;
                break;

                
        }
    }

    public void RoomUserDel(UserSession user)
    {
        //유저 삭제 및 기존 유저 재정렬
        
        _players.TryGetValue(user.m_szUserID, out GameObject toDestroy);
        
        if (toDestroy != null)
        {
            _players.Remove(user.m_szUserID);
            Destroy(toDestroy);
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
