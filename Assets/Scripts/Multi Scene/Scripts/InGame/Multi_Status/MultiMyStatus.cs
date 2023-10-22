using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class MultiMyStatus : MonoBehaviour
{
    public Image playerHpImage;
    public string myplayerName = "";
    public TextMeshProUGUI mynameText;
    public Canvas mystatus;
    public GameObject mynameStatusPrefab;
    private Camera _cam;
    private bool isDestroyed = false;
    private MultiPlayerHealth _playerHealth;
    private Quaternion rotation = new (0, 0, 0, 0);
    
    public void CreateMyStatus(string myPlayerName, Vector3 playerPosition)
    {
        // 각 캐릭터마다 자신만의 UI 요소를 생성하고 위치를 조정
        GameObject nameStatus = Instantiate(mynameStatusPrefab, playerPosition + new Vector3(0, 1, 0), Quaternion.identity);
        MultiMyStatus teamStatus = nameStatus.GetComponent<MultiMyStatus>();
        teamStatus.myplayerName = myplayerName;
        teamStatus.mynameText = nameStatus.GetComponentInChildren<TextMeshProUGUI>();
        teamStatus.mynameText.text = myplayerName;
        //플레이어 체력 관련
        
        MultiScene.Instance._players.TryGetValue(myPlayerName,out var playerPrefab);
        if (playerPrefab != null) _playerHealth = playerPrefab.GetComponent<MultiPlayerHealth>();
        if (_playerHealth != null)
        {
            Debug.Log(myPlayerName+ " 체력바 생성");
        }
        else
        {
            Debug.LogError(myPlayerName + "체력바 생성에 실패했습니다.");
        }
        Transform hpImageTransform = mynameStatusPrefab.transform.Find("Hp Image");
        if (hpImageTransform != null)
        {
            teamStatus.playerHpImage = hpImageTransform.GetComponent<Image>();
        }
        else
        {
            Debug.LogError("Hp Image not found as a child of mynameStatusPrefab.");
        }
        
        // 상태창의 회전을 고정,캔버스도 회전을 막아야 함 
        teamStatus.transform.rotation = new Quaternion(0, 180, 0, 0);
        mystatus.transform.rotation = new Quaternion(0, 180, 0, 0);
        
        // mystatus Canvas의 자식으로 추가
        nameStatus.transform.SetParent(mystatus.transform);
        nameStatus.transform.rotation = new Quaternion(0, 180, 0, 0);

    }

    void Start()
    {
        _cam = Camera.main;
        rotation = mystatus.transform.rotation;

        // mystatus 초기화
        mystatus = GameObject.FindGameObjectWithTag("MyStatus").GetComponent<Canvas>();
        if (mystatus == null)
        {
            Debug.LogError("캔버스를 불러올 수 없습니다.");
        }
        mynameStatusPrefab = Resources.Load<GameObject>("Mystatus");
        Transform hpImageTransform = mynameStatusPrefab.transform.Find("Hp Image");
        if (hpImageTransform != null)
        {
            playerHpImage = hpImageTransform.GetComponent<Image>();
        }
        else
        {
            Debug.LogError("해당 프리팹에서 Hp Image를 찾을 수 없습니다.");
        }

    }

    private void Update()
    {
        if (!isDestroyed && mystatus != null)
        {
            mystatus.transform.rotation = rotation;
        }
    }

    public void UpdatePlayerHp()
    {
        if (_playerHealth != null && playerHpImage != null)
        {
            playerHpImage.fillAmount = (float)_playerHealth.CurrentHealth / _playerHealth.MaxHealth;    
        }
        else
        {
            Debug.LogWarning("플레이어의 체력을 찾을 수 없거나 HP Image를 찾을수 없습니다.");
        }
        
        //playerHpImage.fillAmount = (float)_playerHealth.CurrentHealth / _playerHealth.MaxHealth;    
    }
    public void Awake()
    {
        mystatus = GameObject.FindGameObjectWithTag("MyStatus").GetComponent<Canvas>();
        mynameStatusPrefab = Resources.Load<GameObject>("Mystatus");
    }

    private void OnDestroy()
    {
        isDestroyed = true;
    }
}
