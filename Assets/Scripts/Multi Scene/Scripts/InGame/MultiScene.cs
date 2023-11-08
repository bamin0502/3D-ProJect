using System;
using System.Collections.Generic;
using Cinemachine;
using mino;
using MNF;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEngine.Rendering.Universal;
using Cinemachine.PostFX;
using UnityEngine.AI;


public enum DataType
{
    PlayerAnimation = 1,
    PlayerMovement = 2,
    PlayerPickItem = 3,
    PlayerPickWeapon = 4,
    PlayerHpPlayer = 5,
    PlayerThrownWeapon = 6,
    EnemyAnimation= 7,
    EnemyItem = 8,
    PlayerSkill = 9,
    PlayerUseItem = 10,
    SECOND_CUTSCENE= 11,
    LAST_CUTSCENE= 12,
    PlayerTakeDamage = 13,
    EnemyChaseTarget = 14,
    PlayerDead = 15,
    BossAnimation = 16,
    TARGET_SET = 17,
    EnemySkill = 18,
    EnemyTakeDamage = 19,
    
}
public class MultiScene : MonoBehaviour
{
    #region 메서드
    public static MultiScene Instance;
                                                        
    [SerializeField]public GameObject bossObject;

    public readonly Dictionary<string, GameObject> _players = new();
    [HideInInspector] public List<GameObject> weaponsList; //무기 객체들
    [HideInInspector] public List<GameObject> enemyList; //적 객체들
    [HideInInspector] public List<GameObject> itemsList; //아이템 객체들
    
    public Transform weaponsListParent; //무기 객체들이 있는 부모 객체
    public Transform enemyListParent; //적 객체들이 있는 부모 객체
    public Transform itemListParent; //아이템 객체들이 있는 부모 객체
    public PlayableDirector secondPlayableDirector; //두번째 컷신 감독
    public TimelineAsset secondCut; //두번째 컷신
    public PlayableDirector lastPlayableDirector; //마지막 컷신 감독
    public TimelineAsset lastCut; //마지막 컷신
    public Inventory inventory;
    public TextMeshProUGUI itemUsedText;
    public TextMeshProUGUI noticeText;
    public TextMeshProUGUI coolText;
    public GameObject spaceUI;

    public CinemachineFreeLook cineCam;
    public Camera playerCamera;
    public Camera MinimapCamera;
    public Transform[] positions; //유저 찍어낼 위치
    public GameObject playerPrefab; //찍어낼 유저 프리팹
    public string currentUser = "";

    public TextMeshProUGUI playerNameText; //팀상태창 전용 닉네임 텍스트
    public GridLayoutGroup gridLayoutGroup; //팀상태창 전용 그룹
    public GameObject statusbar; //팀상태창 전용 프리팹

    public TextMeshProUGUI playerMyNameText; //자신 머리위에 닉네임 표시할거 전용 닉네임 텍스트
    public GameObject playerMyStatus; //자신 머리위에 닉네임 표시할거 전용 프리팹
    public Canvas playerMyCanvas; //자신 머리위에 닉네임 표시할거 전용 캔버스
    [HideInInspector] public ThrownWeaponController currentThrownWeaponController;
    public MultiPlayerHealthBar multiPlayerHealthBar;
    public Image[] skillImages;
    public TextMeshProUGUI skillText;
    public ObjectHideCamera objectHideCamera;
    public EnemyCut enemyCut;//두번째 컷신 관련
    public FinalCut finalCut;//마지막 컷신 관련
    public GameObject[] itemPrefabs;
    public bool isMasterClient; //마스터 클라이언트
    public bool isMine;//자기 자신 클라이언트
    private static readonly int AniEnemy = Animator.StringToHash("aniEnemy");
    public GameObject Enemy;
    public MultiPlayerHealth currentPlayerHealth;
    public bool isCutScene = false;
    private string currenViewPlayer = "";
    public Image endingImage; 
    public bool isDead =false;
    [SerializeField]public CinemachineVolumeSettings volumeSettings;
    public NavMeshAgent nav;
    #endregion

    #region 이벤트 함수
    private void Awake()
    {
        if (Instance == null)            
        {
            Instance = this;
        }
        
    }
    //에디터 플레이중에 종료되면 실행시킬거
    private void OnApplicationQuit()
    {
        RevertColor();
    }

    private void Start()
    {
        Instance = this;
        SetUsers();
        SetAllList();
        //해당 방의 첫번째 유저를 마스터 클라이언트로 설정
        isMasterClient = NetGameManager.instance.m_userHandle.m_szUserID.Equals(NetGameManager.instance.m_roomSession
            .m_userList[0].m_szUserID);
        // 만약 현재 클라이언트가 방장 클라이언트라면 isMine은 false
        isMine = !isMasterClient &&
                 // 그렇지 않다면 isMine은 현재 클라이언트가 자신인지를 나타냄
                 NetGameManager.instance.m_userHandle.m_szUserID.Equals(currentUser);
        
        volumeSettings = cineCam.GetComponent<CinemachineVolumeSettings>();
        if (volumeSettings != null)
        {
            RevertColor(); 
        }
    }

    private void Update()
    {
        if (isDead && Input.GetMouseButtonDown(1))
        {
            SwitchToNextPlayer();
        }
    }

    private void LateUpdate()
    {
        if (_players.TryGetValue(currentUser, out GameObject player))
        {
            if (MinimapCamera != null)
            {
                // 플레이어의 위치를 기준으로 미니맵 카메라의 위치를 갱신합니다.
                var position = player.transform.position;
                MinimapCamera.transform.position = new Vector3(position.x, position.y+33, position.z);
            }
        }
    }

    #endregion
    
    public int GetRandomInt(int range)
    {
        //랜덤한 int 값 생성
        if (isMasterClient)
        {
            int rnd = Random.Range(0, range);
            return rnd;
        }

        return -1;
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
                newPlayer.TryGetComponent(out MultiPlayerHealth multiPlayerHealth);
                currentPlayerHealth = multiPlayerHealth;
                multiPlayerHealth.endingImage = endingImage;
                nav = newPlayer.GetComponent<NavMeshAgent>();   
                //아이템 드랍 관련
                pickItem.actionText = itemUsedText;
                pickItem.inventory = inventory;
                //스페이스바 관련
                multiPlayer.coolText = coolText;
                multiPlayer.spaceUI = spaceUI;
                multiPlayer.fill = spaceUI.GetComponent<UnityEngine.UI.Image>();
                //개인체력바 관련
                multiPlayerHealthBar.CreateUiStatus(newPlayerName);
                //카메라 관련
                cineCam.Follow = newPlayer.transform;
                cineCam.LookAt = newPlayer.transform;
                cineCam.GetRig(1).LookAt = newPlayer.transform;
                multiPlayer._camera = playerCamera;
                thrownWeaponController._cam = playerCamera;
                objectHideCamera.tPlayer = newPlayer;
                objectHideCamera.target = newPlayer.transform;
                enemyCut.playerMovement = newPlayer.GetComponent<MultiPlayerMovement>();
                finalCut.playerMovement = newPlayer.GetComponent<MultiPlayerMovement>();
                //미니맵 카메라 관련
                var position = newPlayer.transform.position;
                MinimapCamera.transform.position = new Vector3(position.x, position.y+33, position.z);//Y값만 적절하게 조절하면됩니다.
                //던지는 아이템 관련
                currentThrownWeaponController = thrownWeaponController;
                
                
            }

        }
    }
    
    #region 브로드캐스팅 관련
    public void BroadCastingPlayerSkill()
    {
        var data = new PLAYER_SKILL
        {
            USER = NetGameManager.instance.m_userHandle.m_szUserID,
            DATA = (int)DataType.PlayerSkill,
        };

        string sendData = LitJson.JsonMapper.ToJson(data);
        NetGameManager.instance.RoomBroadcast(sendData);
    }

    public void BroadCastingEnemyTakeDamage(int index, int damage, bool isBoss = false)
    {
        var data = new ENEMY_TAKE_DAMAGE
        {
            USER = NetGameManager.instance.m_userHandle.m_szUserID,
            DATA = (int)DataType.EnemyTakeDamage,
            INDEX = index,
            DAMAGE = damage,
            ISBOSS = isBoss,
        };

        string sendData = LitJson.JsonMapper.ToJson(data);
        NetGameManager.instance.RoomBroadcast(sendData);
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
    public void BroadCastingBossAnimation(int animationNumber, bool isTrigger = false)
    {
        UserSession userSession = NetGameManager.instance.GetRoomUserSession(
            NetGameManager.instance.m_userHandle.m_szUserID);
    
        var data = new BOSS_ANIMATION 
        { 
            USER = userSession.m_szUserID,
            DATA = (int)DataType.BossAnimation,
            ANI_NUM = animationNumber,
            ANI_TYPE = isTrigger,
        };
        
        string sendData = LitJson.JsonMapper.ToJson(data);
        NetGameManager.instance.RoomBroadcast(sendData);

    }
    public void BroadCastingTargetSet(string target)//copy
    {
        UserSession userSession = NetGameManager.instance.GetRoomUserSession(
            NetGameManager.instance.m_userHandle.m_szUserID);
        var data = new TARGET_SET()
        {
            USER = userSession.m_szUserID,
            DATA = (int)DataType.TARGET_SET,
            TARGET_POSITION = target,

        };
        string sendData = LitJson.JsonMapper.ToJson(data);
        NetGameManager.instance.RoomBroadcast(sendData);

    }

    public void BroadCastingEnemyItem(Vector3 destination, int itemIndex = -99)
    {
        if (!isMasterClient) return;
        
        UserSession userSession = NetGameManager.instance.GetRoomUserSession(
            NetGameManager.instance.m_userHandle.m_szUserID);

        var data = new ENEMY_ITEM
        {
            USER = userSession.m_szUserID,
            DATA = (int)DataType.EnemyItem,
            POSITION = VectorToString(destination),
            ITEM_INDEX = itemIndex,
        };

        string sendData = LitJson.JsonMapper.ToJson(data);
        NetGameManager.instance.RoomBroadcast(sendData);
    }
    
    

    public void BroadCastingMovement(Vector3 destination, int target = -99, bool isBoss = false)
    {
        UserSession userSession = NetGameManager.instance.GetRoomUserSession(
            NetGameManager.instance.m_userHandle.m_szUserID);
        
        var data = new PLAYER_MOVE
        {
            USER = userSession.m_szUserID,
            DATA = (int)DataType.PlayerMovement,
            POSITION = VectorToString(destination),
            TARGET = target,
            ISBOSS = isBoss
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

    public void BroadCastingThrowWeapon(Vector3 mousePos, Vector3 playerPos, int skillType)
    {
        UserSession userSession = NetGameManager.instance.GetRoomUserSession(
            NetGameManager.instance.m_userHandle.m_szUserID);

        var data = new THROW_ATTACK
        {
            USER = userSession.m_szUserID,
            DATA = (int)DataType.PlayerThrownWeapon,
            SKILL_TYPE = skillType,
            PLAYER_POSITION = VectorToString(playerPos),
            MOUSE_POSITION = VectorToString(mousePos),
        };

        string sendData = LitJson.JsonMapper.ToJson(data);
        NetGameManager.instance.RoomBroadcast(sendData);
    }

    public void BroadCastingItemUse(int itemType)
    {
        UserSession userSession = NetGameManager.instance.GetRoomUserSession(
            NetGameManager.instance.m_userHandle.m_szUserID);

        var data = new PLAYER_ITEMUSE
        {
            USER = userSession.m_szUserID,
            DATA = (int)DataType.PlayerUseItem,
            ITEM_TYPE = itemType,
        };

        string sendData = LitJson.JsonMapper.ToJson(data);
        NetGameManager.instance.RoomBroadcast(sendData);
    }
    

    public void BroadCastingEnemyAnimation(int index, int animationNumber, bool isTrigger = false)
    {
        UserSession userSession = NetGameManager.instance.GetRoomUserSession(
            NetGameManager.instance.m_userHandle.m_szUserID);

        var data = new ENEMY_ANIMATION
        {
            USER = userSession.m_szUserID,
            DATA = (int)DataType.EnemyAnimation,
            MONSTER_INDEX = index,
            ANI_NUM = animationNumber,
            ANI_TYPE = isTrigger,
        };

        string sendData = LitJson.JsonMapper.ToJson(data);
        NetGameManager.instance.RoomBroadcast(sendData);
    }

    public void BroadCastingSecondCutSceneStart(bool isTrigger = false)
    {
        UserSession userSession= NetGameManager.instance.GetRoomUserSession(
            NetGameManager.instance.m_userHandle.m_szUserID);
        var data = new SECOND_CUTSCENE
        {
            USER = userSession.m_szUserID,
            DATA = (int)DataType.SECOND_CUTSCENE,
            CUTSCENE_NUM=1,
            CUTSCENE_TYPE = isTrigger,
        };
        string sendData = LitJson.JsonMapper.ToJson(data);
        NetGameManager.instance.RoomBroadcast(sendData);
    }

    public void BroadCastingLastCutSceneStart(bool isTrigger = false)
    {
        UserSession userSession= NetGameManager.instance.GetRoomUserSession(
            NetGameManager.instance.m_userHandle.m_szUserID);
        var data = new LAST_CUTSCENE
        {
             USER = userSession.m_szUserID,
             DATA = (int)DataType.LAST_CUTSCENE,
             CUTSCENE_NUM = 2,
             CUTSCENE_TYPE = isTrigger,
        };
        string sendData = LitJson.JsonMapper.ToJson(data);
        NetGameManager.instance.RoomBroadcast(sendData);
    }

    public void BroadCastingTakeDamage(string player, int damage)
    {
        UserSession userSession = NetGameManager.instance.GetRoomUserSession(
            NetGameManager.instance.m_userHandle.m_szUserID);
        
        var data = new PLAYER_TAKE_DAMAGE
        {
            USER = userSession.m_szUserID,
            DATA = (int)DataType.PlayerTakeDamage,
            TARGET = player,
            DAMAGE = damage,
        };
        
        string sendData = LitJson.JsonMapper.ToJson(data);
        NetGameManager.instance.RoomBroadcast(sendData);
    }

    public void BroadCastingPlayerDead(string player)
    {
        UserSession userSession = NetGameManager.instance.GetRoomUserSession(
            NetGameManager.instance.m_userHandle.m_szUserID);
        var data = new PLAYER_DEAD
        {
            USER = userSession.m_szUserID,
            DATA = (int)DataType.PlayerDead,
            TARGET = player,
        };
        string sendData = LitJson.JsonMapper.ToJson(data);
        NetGameManager.instance.RoomBroadcast(sendData);
        
    }

    public void BroadCastingSetEnemyTarget(string player, int index)
    {
        UserSession userSession = NetGameManager.instance.GetRoomUserSession(
            NetGameManager.instance.m_userHandle.m_szUserID);
        var data = new ENEMY_CHASE
        {
            USER = userSession.m_szUserID,
            DATA = (int)DataType.EnemyChaseTarget,
            TARGET = player,
            ENEMY_INDEX = index,
        };
        string sendData = LitJson.JsonMapper.ToJson(data);
        NetGameManager.instance.RoomBroadcast(sendData);
    }
    
    public void BroadCastingSkill(int skillNum)
    {
        UserSession userSession = NetGameManager.instance.GetRoomUserSession(
            NetGameManager.instance.m_userHandle.m_szUserID);
        var data = new ENEMY_SKILL
        {
            USER = userSession.m_szUserID,
            DATA = (int)DataType.EnemySkill,
            SKILL_NUM = skillNum,
        };
        string sendData = LitJson.JsonMapper.ToJson(data);
        NetGameManager.instance.RoomBroadcast(sendData);
    }
    

    #endregion



    #region 유저 관련
    public void RoomUserDel(UserSession user)
    {

        if (_players.TryGetValue(user.m_szUserID, out GameObject toDestroy))
        {
            _players.Remove(user.m_szUserID);
            Destroy(toDestroy);
            MultiTeamstatus teamStatus = statusbar.GetComponent<MultiTeamstatus>();
            teamStatus.DestroyTeamStatus(user.m_szUserID);
        }

        isMasterClient = NetGameManager.instance.m_userHandle.m_szUserID.Equals(NetGameManager.instance.m_roomSession
            .m_userList[0].m_szUserID);
    }

    private void SetAllList()
    {
        //weaponsList.Capacity = weaponsListParent.childCount;
        //enemyList.Capacity = enemyListParent.childCount;
        weaponsList = new List<GameObject>(weaponsListParent.childCount);
        enemyList = new List<GameObject>(enemyListParent.childCount);
        itemsList = new List<GameObject>(itemListParent.childCount);
        
        for (var i = 0; i < weaponsListParent.childCount; i++)
        {
            Transform child = weaponsListParent.GetChild(i);
            weaponsList.Add(child.gameObject);
        }

        for (var i = 0; i < enemyListParent.childCount; i++)
        {
            Transform child = enemyListParent.GetChild(i);
            enemyList.Add(child.gameObject);
        }

        for (var i = 0; i < itemListParent.childCount; i++)
        {
            Transform child = itemListParent.GetChild(i);
            itemsList.Add(child.gameObject);
        }

        for (var i = 0; i < enemyListParent.childCount; i++)
        {
            Transform child = enemyListParent.GetChild(i);
            enemyList.Add(child.gameObject);
        }

    }

    private static string VectorToString(Vector3 position)
    {
        string result = $"{position.x},{position.y},{position.z}";
        return result;
    }

    private static Vector3 StringToVector(string position)
    {
        string[] posString = position.Split(',');
        if (posString.Length < 3) {
            // 예외 상황에 대한 처리
            Debug.LogError("잘못된 위치 문자열입니다.: " + position);
            return Vector3.zero;
        }

        float.TryParse(posString[0], out var x);
        float.TryParse(posString[1], out var y);
        float.TryParse(posString[2], out var z);
    
        return new Vector3(x, y, z);
    }
                             

    #endregion

    #region 카메라 관전 관련

    public void SwitchToNextPlayer()
    {
        endingImage.gameObject.SetActive(false);
        
        noticeText.text = "플레이어 전환(마우스 오른키)";
        List<string> playerKeys = new List<string>(_players.Keys);
        if (string.IsNullOrWhiteSpace(currenViewPlayer))
        {
            currenViewPlayer = playerKeys[0];
        }
        else
        {
            int currentIndex = playerKeys.IndexOf(currenViewPlayer);
            int nextIndex = (currentIndex + 1) % playerKeys.Count;
            currenViewPlayer = playerKeys[nextIndex];
        }

        ChangeCamaraView(currenViewPlayer);
    }
    
    private void ChangeCamaraView(string id)
    {
        _players.TryGetValue(id, out GameObject newPlayer);
        if (!newPlayer)
        {
            Debug.LogError("카메라를 찾을 수 없습니다.");
            return;
        }

        cineCam.Follow = newPlayer.transform;
        cineCam.LookAt = newPlayer.transform;
    }

    public void ChangeColor()
    {
        // colorAdjustments 객체를 가져옵니다.
        if(volumeSettings.m_Profile.TryGet<ColorAdjustments>(out var colorAdjustments))
        {
            // Saturation 값을 -100으로 설정하여 모든 색상을 회색조로 만듭니다.
            colorAdjustments.saturation.value = -100f;
        }
    }

    public void RevertColor()
    {
        // colorAdjustments 객체를 가져옵니다.
        if(volumeSettings.m_Profile.TryGet<ColorAdjustments>(out var colorAdjustments))
        {
            // Saturation 값을 0으로 설정해서 다시 되돌림
            colorAdjustments.saturation.value = 0f;
        }
    }
    #endregion

    #region 모든 유저에게 실행시킬 브로드캐스트
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
            #region 플레이어 애니메이션 관련
            case (int)DataType.PlayerAnimation:
                int aniNum = Convert.ToInt32(jData["ANI_NUM"].ToString());
                bool aniType = Convert.ToBoolean(jData["ANI_TYPE"].ToString());
                if (aniType == false)
                {
                    user.GetComponent<MultiPlayerMovement>().ChangedState((PlayerState)aniNum);
                }
                else
                {
                    user.GetComponent<MultiPlayerMovement>().SetAnimationTrigger(aniNum);
                }
                break;
            #endregion

            #region 플레이어 움직임 관련
            case (int)DataType.PlayerMovement:
                if (user.TryGetComponent<MultiPlayerMovement>(out var userMove2) && user.TryGetComponent<MultiWeaponController>(out var userAttack)){
                    if (userMove2 != null && userMove2.navAgent.isActiveAndEnabled)
                    {
                        int target = Convert.ToInt32(jData["TARGET"].ToString());
                        Vector3 position = StringToVector(jData["POSITION"].ToString());
                        bool bossAttack = Convert.ToBoolean(jData["ISBOSS"].ToString());

                        if (userMove2.navAgent.isOnNavMesh)
                        {
                            if (userAttack == null)
                            {
                                Debug.LogError("MultiWeaponController 컴포넌트를 찾을 수 없습니다.");
                            }
                            else
                            {
                                if (target <= -1)
                                {
                                    userAttack.ClearTarget();
                                    userMove2.navAgent.SetDestination(position);
                                }
                                else
                                {
                                    if (bossAttack)
                                    {
                                        userAttack.SetTarget(target, true);
                                        userMove2.navAgent.SetDestination(position);
                                    }
                                    else
                                    {
                                        userAttack.SetTarget(target);
                                        userMove2.navAgent.SetDestination(position);
                                    }
                                }
                            }
                        }
                    } 
                }
                break;
                

            #endregion

            #region 플레이어 아이템 드랍 관련
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
                int skillType = Convert.ToInt32(jData["SKILL_TYPE"].ToString());
                user.TryGetComponent(out ThrownWeaponController throwWeapon);

                if (skillType == 0)
                {
                    throwWeapon.ThrowGrenade(StringToVector(mousePosition),
                        StringToVector(playerPosition));
                }
                else if (skillType == 1)
                {
                    throwWeapon.SetMousePos(StringToVector(mousePosition));
                    throwWeapon.BowSkill();
                }

                break;
                
            #endregion

            #region 몬스터 애니메이션 관련
            case (int)DataType.EnemyAnimation:
                int monsterIndex = Convert.ToInt32(jData["MONSTER_INDEX"].ToString());
                if (monsterIndex >= 0 && monsterIndex < enemyList.Count && enemyList[monsterIndex] != null)
                {
                    var enemyObject = enemyList[monsterIndex];
                    var multiEnemyComponent = enemyObject.GetComponent<MultiEnemy>();
                    if (multiEnemyComponent != null)
                    {
                        int aniNumber = Convert.ToInt32(jData["ANI_NUM"].ToString());
                        bool monsterAniType = Convert.ToBoolean(jData["ANI_TYPE"].ToString());
                        if (monsterAniType) multiEnemyComponent.SetEnemyAnimation(aniNumber, isTrigger: true);
                        else multiEnemyComponent.SetEnemyAnimation(AniEnemy, aniNumber);
                    }
                }
                break;
                

            #endregion

            #region 몬스터 아이템 드랍 관련
            case (int)DataType.EnemyItem:
                int itemIndex = Convert.ToInt32(jData["ITEM_INDEX"].ToString());
                Vector3 enemyItemPos = StringToVector(jData["POSITION"].ToString());
                var newItem = Instantiate(itemPrefabs[itemIndex], enemyItemPos, quaternion.identity);
                newItem.transform.SetParent(itemListParent);
                itemsList.Add(newItem);
                break;
                

            #endregion

            #region 플레이어 무기 스킬 관련
            case (int)DataType.PlayerSkill:
                user.TryGetComponent(out MultiPlayerSkill multiPlayerSkill);
                multiPlayerSkill.Skill(userID);
                break;
                

            #endregion
           
            #region 플레이어 아이템 사용 관련
            case (int)DataType.PlayerUseItem:
                int itemType = Convert.ToInt32(jData["ITEM_TYPE"].ToString());
                user.TryGetComponent(out MultiPlayerHealth playerHp);
                
                if (itemType == (int)Item.ItemType.Buff)
                {
                    playerHp.MaxHealth += 500;
                }
                else if (itemType == (int)Item.ItemType.Used)
                {
                    playerHp.CurrentHealth += 500;
                    if (playerHp.CurrentHealth >= playerHp.MaxHealth)
                    {
                        playerHp.CurrentHealth = playerHp.MaxHealth;
                    }
                }
                playerHp.UpdateHealth();
                break;
            #endregion

            #region 두번째 컷신 관련
            case (int)DataType.SECOND_CUTSCENE:
                int cutSceneNum = Convert.ToInt32(jData["CUTSCENE_NUM"].ToString());
                user.TryGetComponent(out NavMeshAgent navigation);
                navigation.areaMask = NavMesh.AllAreas;
                navigation.speed = 3.5f;
                secondPlayableDirector.playableAsset = secondCut;
                secondPlayableDirector.Play();
                if(secondPlayableDirector.state == PlayState.Playing)
                {
                    // 컷신이 시작되면 BGM을 중지
                    SoundManager.instance.bgmAudioSource.Stop();
                }
                else
                {
                    // 컷신이 끝나면 BGM을 재생
                    SoundManager.instance.bgmAudioSource.Play();
                    
                }
                
                break;
            #endregion

            #region 마지막 컷신 관련
            case (int)DataType.LAST_CUTSCENE:
                int cutSceneNum2 = Convert.ToInt32(jData["CUTSCENE_NUM"].ToString());
                lastPlayableDirector.playableAsset = lastCut;
                lastPlayableDirector.Play();
                break;
            #endregion

            #region 플레이어 데미지 처리 관련
            case (int)DataType.PlayerTakeDamage:
                int damage = Convert.ToInt32(jData["DAMAGE"].ToString());
                string targetPlayer = jData["TARGET"].ToString();
                if(string.IsNullOrWhiteSpace(targetPlayer)) return;
                _players.TryGetValue(targetPlayer, out GameObject v);
                if(v == null) return;
                v.TryGetComponent(out MultiPlayerHealth playerHealth);
                if (playerHealth != null) playerHealth.TakeDamage(damage);
                break;
            #endregion
            
            #region 플레이어 죽음 관련
            case (int)DataType.PlayerDead:
                string deadPlayer = jData["TARGET"].ToString();
                if(string.IsNullOrWhiteSpace(deadPlayer)) return;
                _players.TryGetValue(deadPlayer, out GameObject deadV);
                if(deadV == null) return;
                deadV.GetComponent<NavMeshAgent>().isStopped = true;
                deadV.TryGetComponent(out MultiPlayerHealth pH);
                if (pH != null)
                {
                    pH.CurrentHealth = 0;
                    pH.UpdateHealth();
                    pH.Die();
                    pH.Invoke(nameof(pH.EndDeath), 3f);
                }
                break;
            #endregion

            #region 몬스터 플레이어 추적 관련
            case (int)DataType.EnemyChaseTarget:
                int enemyIndex = Convert.ToInt32(jData["ENEMY_INDEX"].ToString());
                string chasePlayer = jData["TARGET"].ToString();
                
                if (enemyIndex >= 0 && enemyIndex < enemyList.Count && enemyList[enemyIndex] != null)
                {
                    var enemyObject = enemyList[enemyIndex];
                    var multiEnemyComponent = enemyObject.GetComponent<MultiEnemy>();
                    if(multiEnemyComponent != null)
                    {
                        if (string.IsNullOrWhiteSpace(chasePlayer))
                        {
                            multiEnemyComponent.SetDestination(null);
                            return;
                        }
                        _players.TryGetValue(chasePlayer, out GameObject value);
                        if (value == null)
                        {
                            multiEnemyComponent.SetDestination(null);
                            return;
                        }
                        multiEnemyComponent.SetDestination(value.transform);
                    }
                }
                break;
                

            #endregion
            
            #region 보스 애니메이션
            case (int)DataType.BossAnimation:
                if (bossObject == null) return;
                if (bossObject.TryGetComponent<MultiBoss>(out var boss))
                {
                    int aniBossNum = Convert.ToInt32(jData["ANI_NUM"].ToString());
                    bool bossAniType = Convert.ToBoolean(jData["ANI_TYPE"].ToString());
                    if (bossAniType)
                    {
                        boss.anim.SetTrigger(aniBossNum);
                    }
                }
                break;
            #endregion
            
            #region 보스 추적 
            case (int)DataType.TARGET_SET:
                if (bossObject == null) return;
                string targetName = jData["TARGET_POSITION"].ToString();
                if(string.IsNullOrEmpty(targetName)) return;
                _players.TryGetValue(targetName, out GameObject playerObject);
                if (bossObject.TryGetComponent<MultiBoss>(out var b))
                {
                    b.target = playerObject;
                }
                break;
            #endregion
            
            #region 보스 스킬 
            case (int)DataType.EnemySkill:
                int skillNum = Convert.ToInt32(jData["SKILL_NUM"].ToString());
                bossObject.TryGetComponent<MultiBoss>(out var bossSkill);
                
                if(bossSkill == null) return;
                
                switch (skillNum)
                {
                    case 0:
                        bossSkill.LaunchMissile();
                        break;
                    case 1:
                        bossSkill.Heal();
                        break;
                    case 2:
                        bossSkill.Taunt();
                        break;
                    case 3:
                        bossSkill.DestroyBoss();
                        break;
                }
                break;
            #endregion

            #region 데미지 처리
            case (int)DataType.EnemyTakeDamage:
                int enemyIdx = Convert.ToInt32(jData["INDEX"].ToString());
                int enemyDamage = Convert.ToInt32(jData["DAMAGE"].ToString());
                bool isBoss = Convert.ToBoolean(jData["ISBOSS"].ToString());

                if (isBoss)
                {
                    if (bossObject == null)
                    {
                        Debug.LogWarning("EnemyTakeDamage bossObject 참조 오류");
                        return;
                    }

                    var bossHealth = bossObject.GetComponent<EnemyHealth>();
                    
                    if(bossHealth == null)
                    {
                        Debug.LogWarning("EnemyTakeDamage bossHealth 참조 오류");
                        return;
                    }

                    bossHealth.TakeDamage(enemyDamage, false);
                    return;
                }
                
                if (enemyIdx < 0 || enemyIdx >= enemyList.Count || enemyList[enemyIdx] == null)
                {
                    Debug.LogWarning("EnemyTakeDamage : 적의 인덱스 참조 오류");
                    return;
                }

                var enemyHealth = enemyList[enemyIdx].GetComponent<EnemyHealth>();

                if (enemyHealth == null)
                {
                    Debug.LogWarning("EnemyTakeDamage : enemyHealth 참조 오류");
                    return;
                }

                enemyHealth.TakeDamage(enemyDamage, false);
                break;
                
                                          
            #endregion
           
        }
    }
    #endregion
}


