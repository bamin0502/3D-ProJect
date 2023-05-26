using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Data;

public class Enemy : MonoBehaviour
{
    public float maxHealth;
    public float curHealth;

    public BoxCollider attackArea;
    public Transform target; // Player의 Transform 컴포넌트
    public bool isChase = false; // 추격 중인지 여부
    public bool isAttack = false; // 공격 중인지 여부
    private NavMeshAgent nav; // NavMeshAgent 컴포넌트
    [SerializeField] private Vector3 origninalPosition;
    private DataManager data;

    public float detectionRadius = 5f;
 

    private Rigidbody rigid;
    private BoxCollider boxcollider;
    private Material mat;
    private Animator anim;
    private DataManager Damage;

    public int tmpDamage = 5; //일단 테스트용 나중에 json이랑 연결해야됨

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxcollider = GetComponent<BoxCollider>();
        mat = GetComponent<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        //이런식으로 참고 가능하니 참고하셈
        target = GameObject.FindGameObjectWithTag("Player").gameObject.transform;       
    }
    private void Start()
    {
        origninalPosition = transform.position;
        DataManager dataManager = FindObjectOfType<DataManager>();
        EnemyStat enemyStat = dataManager.LoadFromJsonEncrypted<EnemyStat>("Enemystat1.json");
        maxHealth = enemyStat.EnemyHealth;
        curHealth = enemyStat.Health;
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
        nav.speed = 1f;
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= detectionRadius)
        {
            Chase();
        }
        else if(distance > detectionRadius)
        {
            Returning();
        }
        if (target == null)
        {
            Returning();
        }

    }
    void Returning()
    {
        anim.SetBool("isWalk", false);
        nav.SetDestination(origninalPosition);
    }

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
            StartCoroutine(Attack());

        }
    }
    IEnumerator Attack()
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
