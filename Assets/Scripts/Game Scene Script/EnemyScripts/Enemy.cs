using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Enemy : MonoBehaviour
{
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

    }
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxcollider = GetComponent<BoxCollider>();
        mat = GetComponent<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        Invoke("ChaseStart", 2);
    }
    public void TakeDamage()
    {
        //DataManager.Inst.SetEnemyAttack(); //null이라 일단 주석처리
        StartCoroutine(OnDamage());
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
        if (nav.enabled)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }

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
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);

        
        yield return new WaitForSeconds(1f); //각 몬스터마다 시간설정을 다르게해 공격속도 조절
        attackArea.enabled = true;

        yield return new WaitForSeconds(3f); //각 몬스터마다 시간설정을 다르게해 공격속도 조절
        attackArea.enabled = false;

        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);
    }
}

