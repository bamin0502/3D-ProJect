using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class MultiMyStatus : MonoBehaviour
{
    public Image playerHpImage;
    public string myplayerName = "";
    public TextMeshProUGUI mynameText;
    public Canvas mystatus;
    [SerializeField]public GameObject mynameStatusPrefab;
    private Camera _cam;
    private bool isDestroyed = false;
    private MultiPlayerHealth _playerHealth;
    private Quaternion rotation = new (0, 0, 0, 0);
    private GameObject nameStatus;
    public Gradient gradient;
    public void CreateMyStatus(string myPlayerName, Vector3 playerPosition)
    {
        // 각 캐릭터마다 자신만의 UI 요소를 생성하고 위치를 조정
        nameStatus = Instantiate(mynameStatusPrefab, playerPosition + new Vector3(0, 1, 0), Quaternion.identity);
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
        Transform playerHpTransform = nameStatus.transform.Find("Hp Image");
        if (playerHpTransform != null)
        {
            playerHpImage = playerHpTransform.GetComponent<Image>(); // playerHpImage를 초기화
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
        rotation = transform.rotation;

        // mystatus 초기화
        mystatus = GameObject.FindGameObjectWithTag("MyStatus").GetComponent<Canvas>();
        if (mystatus == null)
        {
            Debug.LogError("캔버스를 불러올 수 없습니다.");
        }
        GradientColorKey[] colorKeys = new GradientColorKey[6];
        colorKeys[0].color = Color.red;
        colorKeys[0].time = 0.0f;
        colorKeys[1].color = Color.red;
        colorKeys[1].time = 0.25f;
        colorKeys[2].color = Color.yellow;
        colorKeys[2].time = 0.25f;
        colorKeys[3].color = Color.yellow;
        colorKeys[3].time = 0.75f;
        colorKeys[4].color = Color.green;
        colorKeys[4].time = 0.75f;
        colorKeys[5].color = Color.green;
        colorKeys[5].time = 1f;

        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[6];
        for (int i = 0; i < 6; i++)
        {
            alphaKeys[i].alpha = 1.0f;
            alphaKeys[i].time = colorKeys[i].time;
        }

        gradient = new Gradient();
        gradient.SetKeys(colorKeys, alphaKeys);
    }

    private void Update()
    {
        if (!isDestroyed && nameStatus != null)
        {
            nameStatus.transform.rotation = rotation;
        }
    }

    public void UpdatePlayerHp()
    {
        playerHpImage.fillAmount = (float)_playerHealth.CurrentHealth / _playerHealth.MaxHealth;
        float fillAmount = (float)_playerHealth.CurrentHealth / _playerHealth.MaxHealth;
        playerHpImage.fillAmount = fillAmount;
        playerHpImage.color = gradient.Evaluate(fillAmount);
    }
    public void Awake()
    {
        mystatus = GameObject.FindGameObjectWithTag("MyStatus").GetComponent<Canvas>();
    }

    private void OnDestroy()
    {
        isDestroyed = true;
    }
}
