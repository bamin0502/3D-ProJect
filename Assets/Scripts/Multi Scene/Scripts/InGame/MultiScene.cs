using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Data;
using mino;
using MNF;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class MultiScene : MonoBehaviour
{
    public static MultiScene Instance;
    
    private Dictionary<string, GameObject> _players = new ();
    [HideInInspector] public List<GameObject> weaponsList; //무기 객체들
    public Transform weaponsListParent; //무기 객체들이 있는 부모 객체
    
    
    public TextMeshProUGUI noticeText;

    public CinemachineFreeLook cineCam;
    public CameraController playerCamera;
    public Transform[] positions; //유저 찍어낼 위치
    public GameObject playerPrefab; //찍어낼 유저 프리팹
    public string currentUser = "";
    private int currentState = -99;
    private void Start()
    {
        
        Instance = this;
        SetWeaponList();
        SetUsers();
    }

    private void SetUsers()
    {
        RoomSession roomSession = NetGameManager.instance.m_roomSession;
        currentUser = NetGameManager.instance.m_userHandle.m_szUserID;

        for (int i = 0; i < roomSession.m_userList.Count; i++)
        {
            GameObject newPlayer = Instantiate(playerPrefab);
            string newPlayerName = roomSession.m_userList[i].m_szUserID;
            newPlayer.name = newPlayerName;
            newPlayer.transform.position = positions[i].position;

            Debug.Log($"{newPlayerName} {newPlayer} {i}");

            _players.Add(newPlayerName, newPlayer);

            newPlayer.TryGetComponent<MultiPlayerMovement>(out var multiPlayer);

            if (newPlayerName.Equals(currentUser))
            {
                //만약 현재 유저일경우
                playerCamera.player = newPlayer.transform;
                multiPlayer._camera = playerCamera.mainCamera;
                cineCam.Follow = newPlayer.transform;
                cineCam.LookAt = newPlayer.transform;
                cineCam.GetRig(1).LookAt = newPlayer.transform;
            }
        }
    }
    private void SetWeaponList()
    {
        weaponsList.Capacity = weaponsListParent.childCount;

        for (int i = 0; i < weaponsListParent.childCount; i++)
        {
            Transform child = weaponsListParent.GetChild(i);
            weaponsList.Add(child.gameObject);
        }
    }

    public void BroadCastingAnimation(int animationNumber, bool isTrigger = false)
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
            ANI_TYPE = isTrigger,
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

        if(currentUser.Equals(userID)) return;
        
        switch (dataID)
        {
            case 1:
                int aniNum = Convert.ToInt32(jData["ANI_NUM"].ToString());
                bool aniType = Convert.ToBoolean(jData["ANI_TYPE"].ToString());
                _players.TryGetValue(userID, out var user);

                if (user != null)
                {
                    if (aniType == false)
                    {
                        user.GetComponent<MultiPlayerMovement>().ChangedState((PlayerState)aniNum);
                    }
                    else
                    {
                        user.GetComponent<MultiPlayerMovement>().SetAnimationTrigger(aniNum);
                    }
                }
                break;
            
            case 2:
                _players.TryGetValue(userID, out var userMove);
                userMove.TryGetComponent<MultiPlayerMovement>(out var userMove2);
                userMove2.navAgent.SetDestination(StringToVector(jData["POSITION"].ToString()));
                break;
            //플레이어 아이템 드랍 관련 테스트 필요
            case 3:
                _players.TryGetValue(userID,out var userItem);
                userItem.TryGetComponent<Slot>(out var userItem2);
                userItem2.UseItemInSlot(Convert.ToInt32(jData["ITEM"].ToString()));
                break;
            //플레이어 공격 관련 테스트 필요
            case 4:
                _players.TryGetValue(userID, out var userAttack);
                userAttack.TryGetComponent<WeaponController>(out var userAttack2);
                userAttack2.isAttack = true;
                break;
            //플레이어 체력 관련 테스트 필요
            case 5:
                Data.PlayerStat playerStat = new Data.PlayerStat();
                playerStat.Health = Convert.ToInt32(jData["HEALTH"].ToString());
                playerStat.PlayerHealth = Convert.ToInt32(jData["PlayerHealth"].ToString());
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
    
}
