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
    
    Vector3 lookVec;
    Vector3 tauntVec;
    public bool isLook = true;
    public bool isDead;

    public Rigidbody rigid;
    public BoxCollider boxCollider;
    Material mat;
    NavMeshAgent nav;
    Animator anim;

    public BossHealthBar bossHealth;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponent<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").gameObject.transform;

        nav.isStopped = true;

    }
    void FreezeVelocity()
    {

        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;

    }

    void Start()
    {

        StartCoroutine(ThinkRoutine());
        string json = "{\"EnemyHealth\": 300, \"Health\": 300, \"Heal\": 20}";
        EnemyStat enemyStat1 = JsonConvert.DeserializeObject<EnemyStat>(json);
        maxHealth = (int)enemyStat1.EnemyHealth;
        curHealth = (int)enemyStat1.Health;
        Healing = (int)enemyStat1.Heal;
        curHealth = maxHealth;

        //BossHealthBar bossHealthBar = FindObjectOfType<BossHealthBar>(); // BossHealthBar 클래스의 인스턴스 가져오기
        //bossHealthBar.boss = this; // 보스 클래스의 인스턴스를 BossHealthBar 클래스의 boss 변수에 할당
    }

   
    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            StopAllCoroutines();
            return;
        }
       
        if (isLook)
        {
                float h = Input.GetAxisRaw("Horizontal");
                float v = Input.GetAxisRaw("Vertical");
                lookVec = new Vector3(h, 0, v) * 5f;
                transform.LookAt(target.position + lookVec);
        }
        else
        {
                nav.SetDestination(tauntVec);
        }
        
    }
    private IEnumerator ThinkRoutine()
    {
        while (true)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance < detectionRadius)
            {
                yield return StartCoroutine(Think());
            }
            yield return null;
        }
    }
    IEnumerator onDamage(Vector3 reactVec)
    {
        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if(curHealth > 0)
        {
            mat.color = Color.white;
        }
        else
        {
            mat.color = Color.gray;
            gameObject.layer = 11;
            isDead = true;
            nav.enabled = false;
            anim.SetTrigger("doDie");
        }
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
        

        yield return new WaitForSeconds(0.6f);
        GameObject instantMissileB = Instantiate(missile, missilePortB.position, missilePortB.rotation);
        Bullet bossMissileB = instantMissileA.GetComponent<Bullet>();
        bossMissileA.target = target;
        yield return new WaitForSeconds(5f);

        

        StartCoroutine(Think());
    }
    IEnumerator Taunt()
    {

        tauntVec = target.position + lookVec;

        isLook = false;
        nav.isStopped = false;
        boxCollider.enabled = false;
        anim.SetTrigger("doTaunt");
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
            bossHealth.UpdateHealth();
            Debug.Log("보스의 체력이 " + healAmount + "만큼 회복됨, 현재 체력: " + maxHealth);
        }
        else
        {
            Debug.Log("더 이상 회복할 수 없습니다.");
        }

        yield return new WaitForSeconds(5f);
        StartCoroutine(Think());
    }
}
