using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Data;
using Newtonsoft.Json;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
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
    public Transform origin;
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
    private Coroutine _playerDetect;

    private int _index;

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
        _index = MultiScene.Instance.enemyList.IndexOf(this.gameObject);
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
        _playerDetect = StartCoroutine(PlayerDetect());
    }
   
    private IEnumerator PlayerDetect()
    {
        WaitForSeconds wait = new WaitForSeconds(0.3f);
        while (true)
        {   
            RaycastHit[] hits = new RaycastHit[10]; 

           
            int hitCount = Physics.SphereCastNonAlloc(origin.position, radius, origin.forward, hits, maxDistance, targetLayer);

            float closestDistance = Mathf.Infinity;
        
            for (int i = 0; i < hitCount; i++)
            {
                if (hits[i].collider.CompareTag("Player"))
                {
                    float distance = Vector3.Distance(origin.position, hits[i].point);

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPrefab = hits[i].collider.gameObject;
                    }
                }
            }

            if (closestPrefab != null)
            {
                target = closestPrefab.transform;
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
        anim.SetBool(IsWalk, true);
        //MultiScene.Instance.BroadCastingMonsterAnimation(_index,IsWalk, true);
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
        if (_playerDetect == null)
        {
            _playerDetect = StartCoroutine(PlayerDetect());
        }
        
        if (isDead)
        {
            if (attackCoroutine != null)
            {
                anim.SetBool(IsAttack, false);
                //MultiScene.Instance.BroadCastingMonsterAnimation(_index,IsAttack, false);
                StopCoroutine(attackCoroutine);
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
    
    void Chase()
    {
         ChaseStart();
         nav.SetDestination(target.position);       
    }
    
    void Targetting()
    {
       
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


