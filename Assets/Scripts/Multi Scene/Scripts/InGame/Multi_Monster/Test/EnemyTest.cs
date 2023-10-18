using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Data;
using Newtonsoft.Json;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EnemyTest : MonoBehaviour
{
    public enum EnemyDamage
    {
        RedMonster,
        RedSpider,
        GreenSpider
    }
    public enum EnemyType
    {  
        //타입에 따라 오디오 재생을 다르게 하기 위한 enum 선언
        Green,
        Red,
        Box
    }

    public float maxHealth;
    public float curHealth;

    public LayerMask targetLayer;
    public float radius = 1.0f;
    public float maxDistance = 100.0f;
    private Transform origin;
    private GameObject closestPrefab;
    
   
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
    public Animator anim;
    private DataManager Damage;
    public bool isDead = false;
    private Coroutine attackCoroutine;
    public float detectionRadius = 5f;
    public EnemyDamage EnemyDam;
    public EnemyType enemyType;
    #region 아이템 드랍관련 변수 선언 - 제작자 방민호
    //아이템 드랍 관련 
    public GameObject[] itemPrefabs;
    private bool hasDroppedItem = false; // 아이템이 이미 떨어진 상태인지 여부를 나타내는 변수
    
    Collider[] hits = new Collider[5]; 
    
    #endregion
    

    private int _index;
    private float closestDistance = Mathf.Infinity;

    public int tmpDamage; //일단 테스트용 나중에 json이랑 연결해야됨
    private static readonly int DoDie = Animator.StringToHash("doDie");
    private static readonly int IsWalk = Animator.StringToHash("isWalk");
    private static readonly int IsAttack = Animator.StringToHash("isAttack");
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxcollider = GetComponent<BoxCollider>();
        mat = GetComponent<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        
       // target = GameObject.FindGameObjectWithTag("Player").gameObject.transform;
        
    }
    private void Start()
    {
        origin = this.gameObject.transform;
        
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
   
    private IEnumerator PlayerDetect()
    {
        WaitForSeconds wait = new WaitForSeconds(1f);
        while (true)
        {
            Physics.OverlapSphereNonAlloc(origin.position, radius, hits, targetLayer);
            
            foreach (var t in hits)
            {
                if(t == null) continue;
                
                float distance = Vector3.Distance(origin.position, t.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPrefab = t.gameObject;
                }
            }

            if (closestPrefab != null)
            {
                target = closestPrefab.transform;
                Debug.Log(target.name);
            }
            else if (closestPrefab == null)
            {
                Debug.Log("널");
            }
            yield return wait;
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
        anim.SetBool(IsWalk, 
            
            
            
            true);
        //MultiScene.Instance.BroadCastingMonsterAnimation(_index,IsWalk, true);
    }

    void FixedUpdate()
    {
        if (isDead) return;
        FreezeVelocity();

        
    }
    void FreezeVelocity()
    {

        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;

    }
    // Update is called once per frame


    public void StartDetect()
    {
        StartCoroutine(PlayerDetect());
    }
    void Update()
    {
        if (isDead)
        {
            if (attackCoroutine != null)
            {
                anim.SetBool(IsAttack, false);
                //MultiScene.Instance.BroadCastingMonsterAnimation(_index,IsAttack, false);
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
    }
    void Returning()
    {
        anim.SetBool(IsWalk, false);
        //MultiScene.Instance.BroadCastingMonsterAnimation(_index,IsWalk, false);
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
    
  
    
    
    
    
    IEnumerator Attack()
    {
        if (!isDead)
        {
            isChase = false;
            isAttack = true;
            switch (enemyType)
            {
                case EnemyType.Box:
                    SoundManager.instance.PlaySE("Red Monster");
                    break;
                case EnemyType.Green:
                    SoundManager.instance.PlaySE("Green Spider");
                    break;
                case EnemyType.Red:
                    SoundManager.instance.PlaySE("Red Spider");
                    break;
            }

            
            
            anim.SetBool(IsAttack, true);
            //MultiScene.Instance.BroadCastingMonsterAnimation(_index, IsAttack, true);
            
            
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
            anim.SetBool(IsAttack, false);
            //MultiScene.Instance.BroadCastingMonsterAnimation(_index,IsAttack, false);
        }
    }
}
