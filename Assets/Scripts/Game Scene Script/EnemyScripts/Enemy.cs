using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Data;
using Newtonsoft.Json;

public class Enemy : MonoBehaviour
{
    public enum EnemyDamage
    {
        RedMonster,
        RedSpider,
        GreenSpider
    }


    public float maxHealth;
    public float curHealth;
   
   
    public BoxCollider attackArea;
    public Transform target; // Player의 Transform 컴포넌트
    public bool isChase = false; // 추격 중인지 여부
    public bool isAttack = false; // 공격 중인지 여부
    private NavMeshAgent nav; // NavMeshAgent 컴포넌트
    [SerializeField] private Vector3 origninalPosition;
    private DataManager data;
    private Rigidbody rigid;
    private BoxCollider boxcollider;
    private Material mat;
    private Animator anim;
    private DataManager Damage;
    public bool isDead = false;
    private Coroutine attackCoroutine;
    public float detectionRadius = 5f;
    public EnemyDamage EnemyDam;
    #region 아이템 드랍관련 변수 선언 - 제작자 방민호
    //아이템 드랍 관련 
    public GameObject[] itemPrefabs;
    private bool hasDroppedItem = false; // 아이템이 이미 떨어진 상태인지 여부를 나타내는 변수
    #endregion
    

    public int tmpDamage; //일단 테스트용 나중에 json이랑 연결해야됨

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxcollider = GetComponent<BoxCollider>();
        mat = GetComponent<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        
       
        target = GameObject.FindGameObjectWithTag("Player").gameObject.transform;
        
    }
    private void Start()
    {
        origninalPosition = transform.position;
        DataManager dataManager = FindObjectOfType<DataManager>();
        EnemyStat enemyStat = dataManager.LoadFromJsonEncrypted<EnemyStat>("Enemystat1.json");
        maxHealth = enemyStat.EnemyHealth;
        curHealth = enemyStat.Health;
        
        string json = "";

        if (EnemyDam == EnemyDamage.RedMonster)
        {
            json = "{\"damage\": 30}";
        }
        else if (EnemyDam == EnemyDamage.RedSpider)
        {
            json = "{\"damage\": 10}";
        }
        else if (EnemyDam == EnemyDamage.GreenSpider)
        {
            json = "{\"damage\": 20}";
        }
        EnemyStat enemy = JsonConvert.DeserializeObject<EnemyStat>(json);
        tmpDamage = (int)enemy.damage;
         
    }
    public void TakeDamage(Transform target)
    {
        StartCoroutine(OnDamage());
    }
    IEnumerator OnDamage()
    {
        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if (maxHealth > 0)
        {
            mat.color = Color.white;
        }
        else
        {
            mat.color = Color.gray;
            Destroy(gameObject, 4);
            gameObject.layer = 7;
            isChase = false;
            nav.enabled = false;
            anim.SetTrigger("doDie");

        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Chase();
            Debug.Log("Player Detected");
        }
    }
    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }
   
    void FixedUpdate()
    {
        if(isDead) return;
        Targetting();
        FreezeVelocity();

        
    }
    void FreezeVelocity()
    {

        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;

    }
    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            if (attackCoroutine != null)
            {
                anim.SetBool("isAttack", false);
                StopCoroutine(attackCoroutine);
                DropRandomItem();

            }
            return;
        }
        nav.speed = 1f;
        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);

            if (distance <= detectionRadius)
            {
                Chase();
            }
            else if (distance > detectionRadius)
            {
                Returning();
            }
        }


        if (target == null)
        {
            Returning();
        }
        if (Time.timeScale == 0)
        {
            return;
        }
    }
    void Returning()
    {
        anim.SetBool("isWalk", false);
        nav.SetDestination(origninalPosition);
        
    }
    #region 랜덤 아이템 스폰 -제작자 방민호
    private void DropRandomItem()
    {
        if (!hasDroppedItem && itemPrefabs.Length > 0)
        {
            StartCoroutine(DropRandomItemCoroutine());
        }
    }
    private IEnumerator DropRandomItemCoroutine()
    {
        yield return new WaitForSeconds(1); // 몬스터가 사라진 후 일정 시간 대기

        if (hasDroppedItem || itemPrefabs.Length == 0)
        {
            Debug.Log("아이템이 없거나 이미 아이템이 떨어졌습니다.");
            yield break;
        }
        hasDroppedItem = true; // 아이템이 떨어진 상태로 변경

        int randomIndex = Random.Range(0, itemPrefabs.Length); // 랜덤한 인덱스 선택
        GameObject itemPrefab = itemPrefabs[randomIndex]; // 선택된 아이템 프리팹

        Instantiate(itemPrefab, transform.position, Quaternion.identity);
    }
    #endregion
    void Chase()
    {
         ChaseStart();
         nav.SetDestination(target.position);       
    }
    
    void Targetting()
    {
        float targetRadius = 1f;
        float targetRange = 1f;
       
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));
        if (rayHits.Length > 0 && !isAttack)
        {
            attackCoroutine = StartCoroutine(Attack());
        }
    }
    IEnumerator Attack()
    {
        if (!isDead)
        {
            isChase = false;
            isAttack = true;
            anim.SetBool("isAttack", true);
            attackArea.enabled = true;
            yield return new WaitForSeconds(0.2f);
            attackArea.enabled = true;

            bool isPlayer = target.TryGetComponent(out PlayerHealth playerHealth);
            if (isPlayer)
            {
                Debug.Log("데미지 입힘");
                playerHealth.TakeDamage(tmpDamage);
            }

            yield return new WaitForSeconds(1f);
            attackArea.enabled = false;

            yield return new WaitForSeconds(1f);
            isChase = true;
            isAttack = false;
            anim.SetBool("isAttack", false);
        }
    }
}
