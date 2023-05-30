using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Data;
using Newtonsoft.Json;

public class Boss : MonoBehaviour
{
    public int Healing;
    public int maxHealth;
    public int curHealth;
    public Transform target;
    public BoxCollider meleeArea;
    public GameObject bullet;
    public GameObject missile;
    public Transform missilePortA;
    public Transform missilePortB;
    public float detectionRadius = 10f;
    private bool isTargetAlive = true;
    private float distance;
    Vector3 lookVec;
    Vector3 tauntVec;
    public bool isLook = true;
    public bool isDead;
    public int missileDmg;
    public int meleeDmg;
    public EnemyHealth enemyHealth;
    public Rigidbody rigid;
    public BoxCollider boxCollider;
    Material mat;
    NavMeshAgent nav;
    Animator anim;
    private bool isTakingDamage = false;
    public EnemyHealthBar enemy;
    public BossHealthBar bossHealth;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponent<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").gameObject.transform;
        enemyHealth = GetComponent<EnemyHealth>();
        nav.isStopped = true;

    }
    void FreezeVelocity()
    {

        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;

    }

    void Start()
    {
        enemyHealth.maxHealth = curHealth;
        enemyHealth.currentHealth = maxHealth;
        StartCoroutine(ThinkRoutine());
        string json = "{\"Heal\": 20}";
        EnemyStat enemyStat1 = JsonConvert.DeserializeObject<EnemyStat>(json);
        Healing = (int)enemyStat1.Heal;
        curHealth = maxHealth;
    }

   
    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            StopAllCoroutines();
            return;
        }

        if (!isDead)
        {
            if (isLook)
            {
                if (curHealth <= 0)
                {
                    isDead = true;
                    nav.isStopped = true;
                    StopAllCoroutines();
                }
                if (target != null)
                {
                    float h = Input.GetAxisRaw("Horizontal");
                    float v = Input.GetAxisRaw("Vertical");
                    lookVec = new Vector3(h, 0, v) * 5f;
                    transform.LookAt(target.position + lookVec);
                }
                else if (target == null)
                {
                    Debug.Log("플레이어가 사망함");
                    StopAllCoroutines();                    
                    
                    return;
                }
            }
            else
            {
                nav.SetDestination(tauntVec);
            }
        }
    }
    private IEnumerator ThinkRoutine()
    {
        while (true)
        {
            if(isTargetAlive && target != null)
            distance = Vector3.Distance(transform.position, target.position);
            if (distance < detectionRadius)
            {
                yield return StartCoroutine(Think());
            }
            yield return null;
        }
    }

    //IEnumerator onDamage(Vector3 reactVec)
    //{
    //    mat.color = Color.red;
    //    yield return new WaitForSeconds(0.1f);

    //    if (curHealth > 0)
    //    {
    //        mat.color = Color.white;
    //    }
    //    else
    //    {
    //        mat.color = Color.gray;
    //        gameObject.layer = 11;
    //        isDead = true;
    //        nav.enabled = false;
    //        anim.SetTrigger("doDie");
    //        StartCoroutine(DeleteSelf());
    //    }
    //}

    IEnumerator DeleteSelf()
    {
        yield return new WaitForSeconds(2.5f);
        Destroy(gameObject);

    }

    IEnumerator Think()
    {
        yield return new WaitForSeconds(0.1f);

        int ranAction = Random.Range(0, 10);
        switch (ranAction)
        {
            case 0:
               
            case 1:
                StartCoroutine(MissileShot());
                break;
            case 2:
                break;
            case 3:
                StartCoroutine(Heal());
                break;
            case 4:
                StartCoroutine(Taunt());
                break;
            case 5:
                break;
            case 6:
                break;
        }
        float waitTime = 5f;
        yield return new WaitForSeconds(waitTime);
    }
    IEnumerator MissileShot()
    {
        anim.SetTrigger("doShot");
        yield return new WaitForSeconds(0.4f);
        GameObject instantMissileA = Instantiate(missile, missilePortA.position, missilePortA.rotation);
        Bullet bossMissileA = instantMissileA.GetComponent<Bullet>();
        bossMissileA.target = target;
        bool isPlayer = target.TryGetComponent(out PlayerHealth playerHealth);
        if (isPlayer)
        {
            Debug.Log("미사일 A 데미지 입힘");
            playerHealth.TakeDamage(missileDmg);
        }


        yield return new WaitForSeconds(0.6f);
        GameObject instantMissileB = Instantiate(missile, missilePortB.position, missilePortB.rotation);
        Bullet bossMissileB = instantMissileB.GetComponent<Bullet>();
        bossMissileB.target = target;
        if (isPlayer)
        {
            Debug.Log("미사일 B 데미지 입힘");
            playerHealth.TakeDamage(missileDmg);
        }
        yield return new WaitForSeconds(5f);

        

        StartCoroutine(Think());
    }
    public void TakeDamage(Transform target)
    {
       
    }
    IEnumerator Taunt()
    {

        tauntVec = target.position + lookVec;

        isLook = false;
        nav.isStopped = false;
        boxCollider.enabled = false;
        anim.SetTrigger("doTaunt");
        bool isPlayer = target.TryGetComponent(out PlayerHealth playerHealth);
        if (isPlayer)
        {
            Debug.Log("근접 데미지 입힘");
            playerHealth.TakeDamage(meleeDmg);
        }
        yield return new WaitForSeconds(1.5f);
        meleeArea.enabled = true;
        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(5f);
        isLook = true;
        nav.isStopped = true;
        boxCollider.enabled = true;
        StartCoroutine(Think());


    }
    IEnumerator Heal()
    {
        anim.SetTrigger("doBigShot");

        if (maxHealth < curHealth)
        {
            int healAmount = Mathf.Min(Healing, curHealth - maxHealth);
            maxHealth += healAmount;
            enemy.UpdateBossHealth();
            Debug.Log("보스의 체력이 " + healAmount + "만큼 회복됨, 현재 체력: " + maxHealth);
        }
        else
        {
            Debug.Log("더 이상 회복할 수 없습니다.");
        }

        yield return new WaitForSeconds(5f);
        StartCoroutine(Think());
    }
    void OnDestroy()
    {
        isTargetAlive = false;
    }
}
