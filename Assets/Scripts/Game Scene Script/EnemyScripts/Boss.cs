using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Data;
using Newtonsoft.Json;
using UnityEngine.UIElements;

public class Boss : MonoBehaviour
{
    public int Healing;
    public float maxHealth;
    public float currentHealth;

    public Transform target;
    public BoxCollider meleeArea;
    public GameObject bullet;
    public GameObject missile;
    public Transform missilePortA;
    public Transform missilePortB;

    public float detectionRadius = 10f;
    private bool isTargetAlive = true;
    private float distance;

    private Vector3 lookVec;
    private Vector3 tauntVec;

    public bool isLook = true;
    public bool isDead;

    public int missileDmg;
    public int meleeDmg;

    //public EnemyHealth enemyHealth;
    public Rigidbody rigid;
    public BoxCollider boxCollider;
    //public EnemyHealthBar enemyHealthBar;
    private Material mat;
    private NavMeshAgent nav;
    private Animator anim;
    private bool isTakingDamage = false;
    private bool hasPlayedDieSound = false;
    public ParticleSystem MeleeAttack;
    public ParticleSystem Jump;
    public ParticleSystem Healdraw;
    public ParticleSystem draw;
    private static readonly int DoShot = Animator.StringToHash("doShot");
    private static readonly int DoTaunt = Animator.StringToHash("doTaunt");
    private static readonly int DoBigShot = Animator.StringToHash("doBigShot");

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
        //enemyHealth = GetComponent<EnemyHealth>();
        //enemyHealthBar = GetComponentInChildren<EnemyHealthBar>();

        StartCoroutine(ThinkRoutine());
        string json = "{\"Heal\": 20}";
        EnemyStat enemyStat1 = JsonConvert.DeserializeObject<EnemyStat>(json);
        Healing = (int)enemyStat1.Heal;
        
        //currentHealth = enemyHealth.maxHealth;  // 현재 체력을 최대 체력으로 초기화합니다.
        maxHealth = currentHealth;  // 최대 체력도 현재 체력과 동일하게 설정합니다.
        
    }


    // Update is called once per frame
    void Update()
    {
        if (isDead && !hasPlayedDieSound)
        {

            SoundManager.instance.PlaySE("Die");
            StopAllCoroutines();
            hasPlayedDieSound = true;

            return;
        }

        if (!isDead)
        {
            if (isLook)
            {

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
        if (Time.timeScale == 0)
        {
            return;
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
        }
        float waitTime = 5f;
        yield return new WaitForSeconds(waitTime);
    }
    IEnumerator MissileShot()
    {
        anim.SetTrigger(DoShot);
        yield return new WaitForSeconds(0.4f);
        GameObject instantMissileA = Instantiate(missile, missilePortA.position, missilePortA.rotation);
        Bullet bossMissileA = instantMissileA.GetComponent<Bullet>();
        bossMissileA.target = target;       
        yield return new WaitForSeconds(0.6f);
        GameObject instantMissileB = Instantiate(missile, missilePortB.position, missilePortB.rotation);
        Bullet bossMissileB = instantMissileB.GetComponent<Bullet>();
        bossMissileB.target = target;
        SoundManager.instance.PlaySE("Missile");
        yield return new WaitForSeconds(5f);
        StartCoroutine(Think());
    }
    IEnumerator Taunt()
    {
        Jump.Play();
        tauntVec = target.position + lookVec;

        isLook = false;
        nav.isStopped = false;
        boxCollider.enabled = false;
        anim.SetTrigger(DoTaunt);
        BossMelee bossMelee = GetComponentInChildren<BossMelee>();
        if (bossMelee != null && !bossMelee.isAttacking)
        {
            bossMelee.isAttacking = true; // 자식 오브젝트의 isAttacking 값을 변경합니다.

            // ... 근접 공격 동작 수행 ...
            bool isPlayer = target.TryGetComponent(out PlayerHealth playerHealth);
            if (isPlayer)
            {
                SoundManager.instance.PlaySE("Taunt");
                Debug.Log("근접 데미지 입힘");
                playerHealth.TakeDamage(bossMelee.meleeDamage);
            }
            
            bossMelee.isAttacking = false; // 공격이 끝났으므로 값을 초기화합니다.
        }
        yield return new WaitForSeconds(1.5f);
        bool enabled1;

        yield return new WaitForSeconds(0.5f);
        enabled1 = false;
        meleeArea.enabled = enabled1;
        {
            MeleeAttack.Play();
            bossMelee.isAttacking = false; // 자식 오브젝트의 isAttacking 값을 변경합니다.
        }
        yield return new WaitForSeconds(5f);
        isLook = true;
        nav.isStopped = true;
        boxCollider.enabled = true;
        StartCoroutine(Think());
    }
    IEnumerator Heal()
    {
        Healdraw.Play();
        draw.Play();
        anim.SetTrigger(DoBigShot);

        //float previousHealth = enemyHealth.currentHealth;
        //float potentialHealth = Mathf.Clamp(enemyHealth.currentHealth + Healing, 0, maxHealth);
        // healedAmount = potentialHealth - previousHealth;

        // if (healedAmount > 0)
        // {
        //     enemyHealth.currentHealth = potentialHealth;
        //     Debug.Log("보스의 체력이 " + healedAmount + "만큼 회복됨, 현재 체력: " + enemyHealth.currentHealth);
        //     enemyHealthBar.UpdateBossHealth();
        // }
        // else if (healedAmount < 0)
        // {
        //     healedAmount = 0; // 음수 값이면 0으로 설정하여 출력하지 않도록 함
        // }
        // else
        // {
        //     Debug.Log("보스의 체력은 이미 최대치이거나 회복할 수 없습니다.");
        // }
        //
        // if (healedAmount > 0)
        // {
        //     yield return new WaitForSeconds(5f);
        // }
        StartCoroutine(Think());
        yield break;
    }
    void OnDestroy()
    {
        isTargetAlive = false;
    }

}
