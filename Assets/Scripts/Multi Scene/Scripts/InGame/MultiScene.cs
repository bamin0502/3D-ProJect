using System;
using System.Collections.Generic;
using Cinemachine;
using Data;
using mino;
using MNF;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public enum DataType
{
    PlayerAnimation = 1,
    PlayerMovement = 2,
    PlayerPickItem = 3,
    PlayerPickWeapon = 4,
    PlayerHpPlayer = 5,
    PlayerThrownWeapon = 6,
    EnemyTest=7,
    
}
public class MultiScene : MonoBehaviour
{
    public static MultiScene Instance;

    public readonly Dictionary<string, GameObject> _players = new();
    [HideInInspector] public List<GameObject> weaponsList; //무기 객체들
    [HideInInspector] public List<GameObject> enemyList; //적 객체들
    [HideInInspector] public List<GameObject> itemsList; //아이템 객체들
    public Transform weaponsListParent; //무기 객체들이 있는 부모 객체
    public Transform enemyListParent; //적 객체들이 있는 부모 객체
    public Transform itemListParent; //아이템 객체들이 있는 부모 객체

    public Inventory inventory;
    public TextMeshProUGUI itemUsedText;
    public TextMeshProUGUI noticeText;
    public TextMeshProUGUI coolText;
    public GameObject spaceUI;

    public CinemachineFreeLook cineCam;
    public CameraController playerCamera;
    public Transform[] positions; //유저 찍어낼 위치
    public GameObject playerPrefab; //찍어낼 유저 프리팹
    public string currentUser = "";

    public List<Transform> boxSpawnPoint;
    public List<Transform> greenSpawnPoint;
    public List<Transform> redSpawnPoint;

    public TextMeshProUGUI playerNameText; //팀상태창 전용 닉네임 텍스트
    public GridLayoutGroup gridLayoutGroup; //팀상태창 전용 그룹
    public GameObject statusbar; //팀상태창 전용 프리팹

    public TextMeshProUGUI playerMyNameText; //자신 머리위에 닉네임 표시할거 전용 닉네임 텍스트
    public GameObject playerMyStatus; //자신 머리위에 닉네임 표시할거 전용 프리팹
    public Canvas playerMyCanvas; //자신 머리위에 닉네임 표시할거 전용 캔버스
    [HideInInspector] public ThrownWeaponController currentThrownWeaponController;
    public MultiPlayerHealthBar multiPlayerHealthBar;
    public Image[] skillImages;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

        }
    }

    private void Start()
    {
        Instance = this;
        SetUsers();
        SetAllList();
    }

    private void SetUsers()
    {
        RoomSession roomSession = NetGameManager.instance.m_roomSession;
        currentUser = NetGameManager.instance.m_userHandle.m_szUserID;

        for (int i = 0; i < roomSession.m_userList.Count; i++)
        {
            GameObject newPlayer = Instantiate(playerPrefab, transform);
            string newPlayerName = roomSession.m_userList[i].m_szUserID;
            newPlayer.name = newPlayerName;
            newPlayer.transform.position = positions[i].position;

            Debug.Log($"{newPlayerName} {newPlayer} {i}");

            _players.Add(newPlayerName, newPlayer);
            newPlayer.TryGetComponent(out MultiTeamstatus teamStatus);
            newPlayer.TryGetComponent<MultiPlayerMovement>(out var multiPlayer);
            //팀 상태창관련 나만이 아닌 다른 사람도 보여야 하므로 여기다가 작성
            teamStatus.playerName = newPlayer.name;
            teamStatus.gridLayoutGroup = gridLayoutGroup;
            teamStatus.statusbar = statusbar;
            teamStatus.nameText = playerNameText;
            teamStatus.CreateTeamStatus(newPlayerName);
            //자신 머리위에 닉네임과 뒤에는 체력 표시할거 관련
            newPlayer.TryGetComponent(out MultiMyStatus myStatus);
            playerMyCanvas = GameObject.FindGameObjectsWithTag("MyStatus")[i].GetComponent<Canvas>();
            myStatus.myplayerName = newPlayer.name;
            myStatus.mynameText = playerMyNameText;
            myStatus.mynameStatusPrefab = playerMyStatus;
            myStatus.mystatus = playerMyCanvas;

            myStatus.CreateMyStatus(newPlayerName, playerMyCanvas.transform.position);
            if (newPlayerName.Equals(currentUser))
            {
                //만약 현재 유저일경우 실행시킬 것들
                newPlayer.TryGetComponent(out ThrownWeaponController thrownWeaponController);
                newPlayer.TryGetComponent(out MultiItemDropController pickItem);
                newPlayer.TryGetComponent(out MultiPlayerHealth playerHealth);
                //아이템 드랍 관련
                pickItem.actionText = itemUsedText;
                pickItem.inventory = inventory;
                //개인 체력바 관련
                multiPlayerHealthBar.CreateUiStatus();
                //스페이스바 관련
                multiPlayer.coolText = coolText;
                multiPlayer.spaceUI = spaceUI;
                multiPlayer.fill = spaceUI.GetComponent<UnityEngine.UI.Image>();
                //카메라 관련
                cineCam.Follow = newPlayer.transform;
                cineCam.LookAt = newPlayer.transform;
                cineCam.GetRig(1).LookAt = newPlayer.transform;
                playerCamera.player = newPlayer.transform;
                multiPlayer._camera = playerCamera.mainCamera;
                thrownWeaponController._cam = playerCamera.mainCamera;
                currentThrownWeaponController = thrownWeaponController;
            }

        }
    }

    public void BroadCastingAnimation(int animationNumber, bool isTrigger = false)
    {
        UserSession userSession = NetGameManager.instance.GetRoomUserSession(
            NetGameManager.instance.m_userHandle.m_szUserID);

        var data = new PLAYER_ANIMATION
        {
            USER = userSession.m_szUserID,
            DATA = (int)DataType.PlayerAnimation,
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
            DATA = (int)DataType.PlayerMovement,
            POSITION = VectorToString(destination),
            TARGET = target,
        };

        string sendData = LitJson.JsonMapper.ToJson(data);
        NetGameManager.instance.RoomBroadcast(sendData);
    }

    public void BroadCastingPickItem(int index, int itemCount = 1)
    {
        UserSession userSession = NetGameManager.instance.GetRoomUserSession(
            NetGameManager.instance.m_userHandle.m_szUserID);

        var data = new PLAYER_ITEM
        {
            USER = userSession.m_szUserID,
            DATA = (int)DataType.PlayerPickItem,
            ITEM_INDEX = index,
            ITEM_COUNT = itemCount,
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
            DATA = (int)DataType.PlayerPickWeapon,
            WEAPON_INDEX = index,
        };

        string sendData = LitJson.JsonMapper.ToJson(data);
        NetGameManager.instance.RoomBroadcast(sendData);
    }
    public void BroadCastingHpPlayer(int index, int health = 2000, int playerHealth = 2000)
    {
        UserSession userSession = NetGameManager.instance.GetRoomUserSession(
            NetGameManager.instance.m_userHandle.m_szUserID);

        var data = new PLAYER_STATUS
        {
            USER = userSession.m_szUserID,
            DATA = (int)DataType.PlayerHpPlayer,
            HEALTH = health,
            PlayerHealth = playerHealth,
        };

        string sendData = LitJson.JsonMapper.ToJson(data);
        NetGameManager.instance.RoomBroadcast(sendData);
    }
    public void BroadCastingThrowWeapon(Vector3 mousePos, Vector3 playerPos)
    {
        UserSession userSession = NetGameManager.instance.GetRoomUserSession(
            NetGameManager.instance.m_userHandle.m_szUserID);

        var data = new THROW_ATTACK
        {
            USER = userSession.m_szUserID,
            DATA = (int)DataType.PlayerThrownWeapon,
            PLAYER_POSITION = VectorToString(playerPos),
            MOUSE_POSITION = VectorToString(mousePos),
        };

        string sendData = LitJson.JsonMapper.ToJson(data);
        NetGameManager.instance.RoomBroadcast(sendData);
    }



    public void BroadCastingMonsterSpawn(int index, Vector3 destination)
    {
        UserSession userSession = NetGameManager.instance.GetRoomUserSession(
            NetGameManager.instance.m_userHandle.m_szUserID);
        var data = new MONSTER_SPAWN
        {

            DATA = 7,
            MONSTER_CODE = index,
            POSITION = VectorToString(destination)

        };
    }

    public void BroadCastingMonsterAnimation(int index, int aniNum, bool flag)
    {
        UserSession userSession = NetGameManager.instance.GetRoomUserSession(
            NetGameManager.instance.m_userHandle.m_szUserID);
        var data = new ENEMY_ANIMATION
        {
            USER = userSession.m_szUserID,
            DATA = 6,
            MONSTER_INDEX = index,
            ANI_NUM = aniNum,
            ANI_TYPE = flag,
        };

        string sendData = LitJson.JsonMapper.ToJson(data);
        NetGameManager.instance.RoomBroadcast(sendData);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            foreach (GameObject enemy in MultiScene.Instance.enemyList)
            {
                enemy.TryGetComponent<EnemyTest>(out var e);
                if (e != null) e.StartDetect();
            }
        }
    }


    public void RoomUserDel(UserSession user)
    {

        if (_players.TryGetValue(user.m_szUserID, out GameObject toDestroy))
        {
            _players.Remove(user.m_szUserID);
            Destroy(toDestroy);
            MultiTeamstatus teamStatus = statusbar.GetComponent<MultiTeamstatus>();
            teamStatus.DestroyTeamStatus(user.m_szUserID);

        }



    }

    private void SetAllList()
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

        for (int i = 0; i < itemListParent.childCount; i++)
        {
            Transform child = itemListParent.GetChild(i);
            itemsList.Add(child.gameObject);
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

        if (currentUser.Equals(userID)) return;
        _players.TryGetValue(userID, out var user);
        if (user == null) return;

        switch (dataID)
        {
            case (int)DataType.PlayerAnimation:
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
            case (int)DataType.PlayerMovement:
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
                    userMove2.navAgent.SetDestination(StringToVector(jData["POSITION"].ToString()));
                }

                break;
            //플레이어 아이템 드랍 관련 테스트 필요
            case (int)DataType.PlayerPickItem:
                int index = Convert.ToInt32(jData["ITEM_INDEX"].ToString());
                Destroy(itemsList[index]);
                break;
            case (int)DataType.PlayerPickWeapon:
                int weaponIndex = Convert.ToInt32(jData["WEAPON_INDEX"].ToString());
                user.GetComponent<MultiWeaponController>().PickWeapon(weaponIndex);
                break;
            case (int)DataType.PlayerThrownWeapon:
                string playerPosition = jData["PLAYER_POSITION"].ToString();
                string mousePosition = jData["MOUSE_POSITION"].ToString();
                user.GetComponent<ThrownWeaponController>().ThrowGrenade(StringToVector(mousePosition), StringToVector(playerPosition));
                break;
            case (int)DataType.EnemyTest:
                int monsterIndex = Convert.ToInt32(jData["MONSTER_INDEX"].ToString());
                int aniNumber = Convert.ToInt32(jData["ANI_NUM"].ToString());
                bool monsterAniType = Convert.ToBoolean(jData["ANI_TYPE"].ToString());
                enemyList[monsterIndex].TryGetComponent<Enemy>(out var e);
                e.anim.SetBool(aniNumber, monsterAniType);
                break;
            // case 7:
            //     int monstercode = Convert.ToInt32(jData["MONSTER_INDEX"].ToString());
            //     
            //
            //     break;
        }
    }
}
