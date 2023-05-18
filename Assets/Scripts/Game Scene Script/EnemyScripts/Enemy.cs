using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Enemy : MonoBehaviour
{
    public float detectionRadius = 5f; // Player를 감지할 범위
    public float returnSpeed = 5f; // 본래 자리로 돌아가는 속도

    private Transform player; // Player의 Transform 컴포넌트
    private Vector3 originalPosition; // 본래 자리의 위치
    private bool isReturning = false; // 본래 자리로 돌아가는 중인지 여부
    public int maxHealth;
    public int curHealth;
    public Transform target;
    public bool isChase;
    public BoxCollider attackArea;
    public bool isAttack;

    private Rigidbody rigid;
    private BoxCollider boxcollider;
    private Material mat;
    private NavMeshAgent nav;
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Player를 찾아서 Transform을 가져옴
        originalPosition = transform.position; // 본래 자리의 위치를 저장
        Invoke("ChaseStart", 1);

    }
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxcollider = GetComponent<BoxCollider>();
        mat = GetComponent<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hammer")
        {
            //웨폰 스크립트 참조하기
            DataManager.Inst.SetEnemyAttack();
            OnDamage();
        }
        else if (other.tag == "Bullet")
        {
            //웨폰 스크립트 참조하기
            DataManager.Inst.SetEnemyAttack();
            OnDamage();
        }
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
        float distance = Vector3.Distance(transform.position, player.position);
        if (nav.enabled)
        {
            if (distance <= detectionRadius)
            {
                

                if (!isReturning)
                {
                    
                    nav.SetDestination(target.position);
                    nav.isStopped = !isChase;
                }
            }
            else if (isReturning) // 추적 중이 아니고 본래 자리로 돌아가는 중인 경우
            {
                // Enemy Object의 이동 방향을 본래 자리로 설정
                Vector3 direction = (originalPosition - transform.position).normalized;
                transform.position += direction * Time.deltaTime * returnSpeed;

                // 본래 자리에 도달하면 본래 자리로 돌아왔으므로 돌아가는 중인 플래그를 해제
                if (Vector3.Distance(transform.position, originalPosition) < 0.1f)
                {
                    isReturning = false;
                }
            }
        }

    }
    public void ReturnToOriginalPosition()
    {
        isReturning = true;
    }
    void FreezeVelocity()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        
        
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
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);

        
        yield return new WaitForSeconds(3f); //각 몬스터마다 시간설정을 다르게해 공격속도 조절
        attackArea.enabled = true;
        
        yield return new WaitForSeconds(3f); //각 몬스터마다 시간설정을 다르게해 공격속도 조절
        attackArea.enabled = false;

        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);
    }
}

