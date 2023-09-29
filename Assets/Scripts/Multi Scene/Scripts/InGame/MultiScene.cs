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
using UnityEngine.UI;

public class MultiScene : MonoBehaviour
{
    public static MultiScene Instance;
    
    private readonly Dictionary<string, GameObject> _players = new ();
    [HideInInspector] public List<GameObject> weaponsList; //무기 객체들
    public List<GameObject> enemyList; //적 객체들
    public Transform weaponsListParent; //무기 객체들이 있는 부모 객체
    public Transform enemyListParent; //적 객체들이 있는 부모 객체
    
    public List<GameObject> itemsList; //아이템 객체들
   
    
    public TextMeshProUGUI noticeText;

    public CinemachineFreeLook cineCam;
    public CameraController playerCamera;
    public Transform[] positions; //유저 찍어낼 위치
    public GameObject playerPrefab; //찍어낼 유저 프리팹
    public string currentUser = "";
    private void Start()
    {
        Instance = this;
        SetUsers();
        SetWeaponAndEnemyList();
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
    private void SetWeaponAndEnemyList()
    {
        weaponsList.Capacity = weaponsListParent.childCount;
        enemyList.Capacity = enemyListParent.childCount;
        
        for (int i = 0; i < weaponsListParent.childCount; i++)
        {
            Transform child = weaponsListParent.GetChild(i);
            weaponsList.Add(child.gameObject);
        }

        for (int i = 0; i < enemyListParent.childCount; i++)
        {
            Transform child = enemyListParent.GetChild(i);
            enemyList.Add(child.gameObject);
        }
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
        _players.TryGetValue(userID, out var user);
        if (user == null) return;
        
        switch (dataID)
        {
            case 1:
                int aniNum = Convert.ToInt32(jData["ANI_NUM"].ToString());
                bool aniType = Convert.ToBoolean(jData["ANI_TYPE"].ToString());

                if (aniType == false)
                {
                    user.GetComponent<MultiPlayerMovement>().ChangedState((PlayerState)aniNum);
                }
                else
                {
                    int BowAttack = Animator.StringToHash("BowAttack");
                    user.TryGetComponent<MultiMeleeWeaponController>(out var userBowAttack);
                    if (aniNum == BowAttack) userBowAttack.CoroutineArrow();
                    user.GetComponent<MultiPlayerMovement>().SetAnimationTrigger(aniNum);
                }
                break;
            case 2:
                user.TryGetComponent<MultiPlayerMovement>(out var userMove2);
                user.TryGetComponent<MultiWeaponController>(out var userAttack);
                int target = Convert.ToInt32(jData["TARGET"].ToString());

                if (target <= -1)
                {
                    userAttack.ClearTarget();
                    userMove2.navAgent.SetDestination(StringToVector(jData["POSITION"].ToString()));
                }
                else
                {
                    userAttack.SetTarget(target);
                }
                break;
            //플레이어 아이템 드랍 관련 테스트 필요
            case 3:
                string itemIndex = Convert.ToString(jData["ITEM_INDEX"].ToString());
                user.GetComponent<ItemPickup>().name = itemIndex;
                break;
            //플레이어 공격 관련 테스트 필요
            case 4:
                user.TryGetComponent<WeaponController>(out var userAttack2);
                userAttack2.isAttack = true;
                break;
            //플레이어 체력 관련 테스트 필요
            case 5:
                Data.PlayerStat playerStat = new Data.PlayerStat
                {
                    Health = Convert.ToInt32(jData["HEALTH"].ToString()),
                    PlayerHealth = Convert.ToInt32(jData["PlayerHealth"].ToString())
                };
                break;
            case 6:
                int weaponIndex = Convert.ToInt32(jData["WEAPON_INDEX"].ToString());
                user.GetComponent<MultiWeaponController>().PickWeapon(weaponIndex);
                break;
                
        }
    }

    #region 브로드캐스팅 관련

    public void BroadCastingAnimation(int animationNumber, bool isTrigger = false)
    {
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

    public void BroadCastingMovement(Vector3 destination, int target = -99)
    {
        UserSession userSession = NetGameManager.instance.GetRoomUserSession(
            NetGameManager.instance.m_userHandle.m_szUserID);

        var data = new PLAYER_MOVE
        {
            USER = userSession.m_szUserID,
            DATA = 2,
            POSITION = VectorToString(destination),
            TARGET = target,
        };

        string sendData = LitJson.JsonMapper.ToJson(data);
        NetGameManager.instance.RoomBroadcast(sendData);
    }

    public void BroadCastingPickWeapon(int index)
    {
        UserSession userSession = NetGameManager.instance.GetRoomUserSession(
            NetGameManager.instance.m_userHandle.m_szUserID);

        var data = new PLAYER_WEAPON
        {
            USER = userSession.m_szUserID,
            DATA = 6,
            WEAPON_INDEX = index,
        };

        string sendData = LitJson.JsonMapper.ToJson(data);
        NetGameManager.instance.RoomBroadcast(sendData);
    }

    #endregion

    public void RoomUserDel(UserSession user)
    {
        _players.TryGetValue(user.m_szUserID, out GameObject toDestroy);
        
        if (toDestroy != null)
        {
            _players.Remove(user.m_szUserID);
            Destroy(toDestroy);
        }
    }

    public void BroadCastingPickItem(int index)
    {
          UserSession userSession= NetGameManager.instance.GetRoomUserSession(
            NetGameManager.instance.m_userHandle.m_szUserID);
                        
          List<ItemProperty> itemProperties = new List<ItemProperty>();

          ItemProperty UsedItem = new ItemProperty
          {
              itemType = Item.ItemType.Used,
              ITEM_IMAGE = itemsList[index].GetComponent<ItemPickup>().item.itemImage,
              ITEM_NAME = "",
              ITEM_PREFAB = itemsList[index].GetComponent<ItemPickup>().item.GameObject(),
              
          };
          itemProperties.Add(UsedItem);
          ItemProperty BuffItem = new ItemProperty
          {
              itemType = Item.ItemType.Buff,
              ITEM_IMAGE = itemsList[index].GetComponent<ItemPickup>().item.itemImage,
              ITEM_NAME = "",
              ITEM_PREFAB = itemsList[index].GetComponent<ItemPickup>().item.GameObject(),
          };
          itemProperties.Add(BuffItem);
          ItemProperty ThrowItem = new ItemProperty
          {
              itemType = Item.ItemType.Throw,
              ITEM_IMAGE = itemsList[index].GetComponent<ItemPickup>().item.itemImage,
              ITEM_NAME = "",
              ITEM_PREFAB = itemsList[index].GetComponent<ItemPickup>().item.GameObject(),
          };
          var data = new PLAYER_ITEM
          {
              USER = userSession.m_szUserID,
              DATA = 3,
              ItemProperties = itemProperties
          };

          
          string sendData = LitJson.JsonMapper.ToJson(data);
          NetGameManager.instance.RoomBroadcast(sendData);
    }
}
