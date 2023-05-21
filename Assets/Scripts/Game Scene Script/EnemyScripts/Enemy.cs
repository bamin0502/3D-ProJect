using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Newtonsoft.Json;
public class Enemy : MonoBehaviour
{
    public float maxHealth;
    public float curHealth;
    
    public BoxCollider attackArea;
    
    public float detectionRadius = 5f; // Player를 감지할 범위
    public float attackRadius = 1f; // 공격할 범위
    public float returnRadius = 20f; // 본래 자리로 돌아갈 범위
    public float returnSpeed = 5f; // 본래 자리로 돌아가는 속도

    public Transform target; // Player의 Transform 컴포넌트
    public bool isChase = false; // 추격 중인지 여부
    
    public bool isAttack = false; // 공격 중인지 여부

    private NavMeshAgent nav; // NavMeshAgent 컴포넌트
    private Vector3 originalPosition; // 본래 자리의 위치
    private bool isReturning = false; // 본래 자리로 돌아가는 중인지 여부

    private Rigidbody rigid;
    private BoxCollider boxcollider;
    private Material mat;
    
    private Animator anim;
    private DataManager Damage;
    // Start is called before the first frame update
    void Start()
    {
       
    }
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxcollider = GetComponent<BoxCollider>();
        mat = GetComponent<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        
        originalPosition = transform.position;
        
    }
    public void TakeDamage()
    {
        //DataManager.Inst.SetEnemyAttack(); //null이라 일단 주석처리
        StartCoroutine(OnDamage());
        Damage.SetEnemyAttack();
    }
    IEnumerator OnDamage()
    {
        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if (curHealth > 0)
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
    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= detectionRadius)
        {
            // 추격 시작
            if (!isChase)
            {
                isChase = true;
                anim.SetBool("isWalk", true);
                nav.SetDestination(target.position);
            }

            // 공격 가능 범위에 들어왔을 때 공격
            if (distance <= attackRadius && !isAttack)
            {
                isChase = false;
                StartCoroutine(Attack());
            }
        }
        else if (isChase)
        {
            // 본래 위치로 돌아가기 시작
            if (!isReturning)
            {
                StartCoroutine(ReturnToOriginalPosition());
            }
        }

}
IEnumerator ReturnToOriginalPosition()
    {
        isReturning = true;
        nav.SetDestination(originalPosition);

        while (Vector3.Distance(transform.position, originalPosition) > 0.1f)
        {
            yield return null;
        }

        isReturning = false;
        isChase = false;
        anim.SetBool("isWalk", false);
 }

    void FreezeVelocity()
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }

    void FixedUpdate()
    {
        Targetting();
        FreezeVelocity();
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
        
        isAttack = true;
        anim.SetBool("isAttack", true);


        yield return new WaitForSeconds(1f); //각 몬스터마다 시간설정을 다르게해 공격속도 조절
        attackArea.enabled = true;

        yield return new WaitForSeconds(3f); //각 몬스터마다 시간설정을 다르게해 공격속도 조절
        attackArea.enabled = false;

       
        isAttack = false;
        anim.SetBool("isAttack", false);
    }
}