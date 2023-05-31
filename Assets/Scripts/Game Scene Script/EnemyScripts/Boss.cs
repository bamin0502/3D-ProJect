using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Data;
using Newtonsoft.Json;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Boss : MonoBehaviour
{
    public int Healing;

    public int maxHealth;
    public int currentHealth;

    public Transform target;
    public BoxCollider meleeArea;
    public GameObject bullet;
    public GameObject missile;
    public Transform missilePortA;
    public Transform missilePortB;

    public float detectionRadius = 10f;
    private bool isTargetAlive = true;
    private float distance;
<<<<<<< Updated upstream

    private Vector3 lookVec;
    private Vector3 tauntVec;

=======
    
    Vector3 lookVec;
    Vector3 tauntVec;
>>>>>>> Stashed changes
    public bool isLook = true;
    public bool isDead;

    public int missileDmg;
    public int meleeDmg;

    public EnemyHealth enemyHealth;
    public Rigidbody rigid;
    public BoxCollider boxCollider;
<<<<<<< Updated upstream
    public EnemyHealthBar enemyHealthBar;
    private Material mat;
    private NavMeshAgent nav;
    private Animator anim;
=======
    public AudioClip audioClip;
    private SoundManager soundManager;
    Material mat;
    NavMeshAgent nav;
    Animator anim;
>>>>>>> Stashed changes
    private bool isTakingDamage = false;

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
<<<<<<< Updated upstream
        enemyHealth = GetComponent<EnemyHealth>();
        enemyHealthBar = GetComponentInChildren<EnemyHealthBar>();

=======
        soundManager = SoundManager.instance;
        enemyHealth.maxHealth = curHealth;
        enemyHealth.currentHealth = maxHealth;
>>>>>>> Stashed changes
        StartCoroutine(ThinkRoutine());
        string json = "{\"Heal\": 20}";
        EnemyStat enemyStat1 = JsonConvert.DeserializeObject<EnemyStat>(json);
        Healing = (int)enemyStat1.Heal;

        currentHealth = enemyHealth.maxHealth;  // 현재 체력을 최대 체력으로 초기화합니다.
        maxHealth = currentHealth;  // 최대 체력도 현재 체력과 동일하게 설정합니다.
        
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
        soundManager.PlaySE("Die");
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
<<<<<<< Updated upstream
        bossMissileA.target = target;       
=======
        soundManager.PlaySE("Missile");
        bossMissileA.target = target;
        
      
           
        


>>>>>>> Stashed changes
        yield return new WaitForSeconds(0.6f);
        GameObject instantMissileB = Instantiate(missile, missilePortB.position, missilePortB.rotation);
        Bullet bossMissileB = instantMissileB.GetComponent<Bullet>();
        bossMissileB.target = target;
        soundManager.PlaySE("Missile");
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
        BossMelee bossMelee = GetComponentInChildren<BossMelee>();
        if (bossMelee != null && !bossMelee.isAttacking)
        {
            bossMelee.isAttacking = true; // 자식 오브젝트의 isAttacking 값을 변경합니다.

            // ... 근접 공격 동작 수행 ...
            bool isPlayer = target.TryGetComponent(out PlayerHealth playerHealth);
            if (isPlayer)
            {
                soundManager.PlaySE("Taunt");
                Debug.Log("근접 데미지 입힘");
                playerHealth.TakeDamage(bossMelee.meleeDamage);
            }

            bossMelee.isAttacking = false; // 공격이 끝났으므로 값을 초기화합니다.
        }
        yield return new WaitForSeconds(1.5f);
        meleeArea.enabled = true;
        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;
        {
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
        anim.SetTrigger("doBigShot");

        int previousHealth = enemyHealth.currentHealth;
        int potentialHealth = Mathf.Clamp(enemyHealth.currentHealth + Healing, 0, maxHealth);
        int healedAmount = potentialHealth - previousHealth;

        if (healedAmount > 0)
        {
<<<<<<< Updated upstream
            enemyHealth.currentHealth = potentialHealth;
            Debug.Log("보스의 체력이 " + healedAmount + "만큼 회복됨, 현재 체력: " + enemyHealth.currentHealth);
            enemyHealthBar.UpdateBossHealth();
        }
        else if (healedAmount < 0)
        {
            healedAmount = 0; // 음수 값이면 0으로 설정하여 출력하지 않도록 함
=======
            soundManager.PlaySE("Heal");
            int healAmount = Mathf.Min(Healing, curHealth - maxHealth);
            maxHealth += healAmount;
            enemy.UpdateBossHealth();
            Debug.Log("보스의 체력이 " + healAmount + "만큼 회복됨, 현재 체력: " + maxHealth);
>>>>>>> Stashed changes
        }
        else
        {
            Debug.Log("보스의 체력은 이미 최대치이거나 회복할 수 없습니다.");
        }

        if (healedAmount > 0)
        {
            yield return new WaitForSeconds(5f);
        }
        StartCoroutine(Think());

    }
    void OnDestroy()
    {
        isTargetAlive = false;
    }

}
