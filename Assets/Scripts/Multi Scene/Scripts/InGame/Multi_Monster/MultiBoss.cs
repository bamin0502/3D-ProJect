using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Data;
using Newtonsoft.Json;

public class MultiBoss : MonoBehaviour
{
    private static readonly int DoShot = Animator.StringToHash("doShot");
    private static readonly int DoTaunt = Animator.StringToHash("doTaunt");
    private static readonly int DoBigShot = Animator.StringToHash("doBigShot");
    private static readonly int Dodie = Animator.StringToHash("doDie");

    public ParticleSystem MeleeAttack;
    public ParticleSystem Jump;
    public ParticleSystem Healdraw;
    public ParticleSystem draw;

    public GameObject bullet;
    public GameObject missile;
    public Transform missilePortA;
    public Transform missilePortB;

    //점프 이동
    private float moveSpeed = 5.0f;
    private Vector3 lookVec;
    private Vector3 tauntVec;
    //점프 공격
    private Transform raycastOrigin; //중심 위치
    private float raycastDistance = 2f; // 레이케스트 거리 설정
    public LayerMask hitLayer; // 충돌을 감지할 레이어 설정
    private int damageAmount = 10; // 데미지 양 설정

    public int missileDmg;
    public int meleeDmg;

    public EnemyHealthBar enemyHealthBar;
    public EnemyHealth enemyHealth;
    public int healAmount;
    public float maxHealth;
    public float currentHealth;

    public GameObject target;
    public Transform targetPos;

    public float detectionRadius = 10f;

    //디텍팅 관련 변수
    public Collider[] _targets = new Collider[5];
    private Coroutine _detect;

    public bool isDead;
    private int hitCount;


    public Rigidbody rigid;

    private NavMeshAgent nav;
    public Animator anim;

    private bool _isThink;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        nav = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        string json = "{\"Heal\": 20}";
        EnemyStat enemyStat1 = JsonConvert.DeserializeObject<EnemyStat>(json);
        healAmount = (int)enemyStat1.Heal;
        currentHealth = enemyHealth.maxHealth;
        maxHealth = enemyHealth.maxHealth;
    }

    public IEnumerator PlayerDetect()
    {
        if (!MultiScene.Instance.isMasterClient) yield break;
    
        WaitForSeconds wait = new WaitForSeconds(1f);
        while (true)
        {
            _targets = new Collider[5];
            hitCount = Physics.OverlapSphereNonAlloc(transform.position, detectionRadius, _targets,
                LayerMask.GetMask("Player"));

            Debug.LogWarning(hitCount + "====================");

            if (hitCount <= 0) target = null;
            else if(target == null && hitCount >= 1)
            {
                target = _targets[0].gameObject;
                MultiScene.Instance.BroadCastingTargetSet(target.name);
            }

            yield return wait;
        }
    }
    public IEnumerator ChangeTarget()
    {
        if (!MultiScene.Instance.isMasterClient) yield break;
    
        WaitForSeconds wait = new WaitForSeconds(3f);
        while (true)
        {
            if(hitCount >= 1)
            {
                SelectTarget();
                yield return wait;
            }
        
        
            yield return wait;
        }
    }

    public IEnumerator StartThink()
    {
        if (!MultiScene.Instance.isMasterClient) yield break;
    
        WaitForSeconds wait = new WaitForSeconds(3f);
        while (true)
        {
            if(target == null || _isThink) yield return wait;
            Think();
            yield return wait;
        }
    }
    public void SelectTarget()
    {
        if(!MultiScene.Instance.isMasterClient) return;
        int randomIndex = MultiScene.Instance.GetRandomInt(hitCount - 1);
        target = _targets[randomIndex].gameObject;
        targetPos = target.gameObject.transform;
        MultiScene.Instance.BroadCastingTargetSet(target.name);
    }
    public void Update()
    {
        if (target != null)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookVec = new Vector3(h, 0, v) * 5f;
            transform.LookAt(target.transform.position + lookVec);
        }
    
    }

    public void LaunchMissile()
    {   
        MissileShot(missilePortA);
        MissileShot(missilePortB);
    }



    public void Think()
    {
        int ranAction = MultiScene.Instance.GetRandomInt(6);
        if(target == null) return;
        _isThink = true;
        switch (4)
        {
       case 1:
            LaunchMissile();
            MultiScene.Instance.BroadCastingSkill(0);
            anim.SetTrigger(DoShot);
            MultiScene.Instance.BroadCastingBossAnimation(DoShot,true); 
            break;
        
       case 2:
            break;
       case 3:
            Debug.LogWarning("Heal");
            Heal();
            MultiScene.Instance.BroadCastingSkill(1);
            anim.SetTrigger(DoBigShot);
            MultiScene.Instance.BroadCastingBossAnimation(DoBigShot,true); 
            break;
       case 4:
           Taunt();
           MultiScene.Instance.BroadCastingSkill(2);
           anim.SetTrigger(DoTaunt);
           MultiScene.Instance.BroadCastingBossAnimation(DoTaunt,true);
          
            break;
        /*case 5:
            break;
        case 6:
            break;*/
        }
    }
    public void MissileShot(Transform obj)
    {
        if (target != null)
        {
            GameObject instantMissileA = Instantiate(missile, obj.position, missilePortA.rotation);
            MultiBullet bossMissileA = instantMissileA.GetComponent<MultiBullet>();
            bossMissileA.target = target.transform;
            SoundManager.instance.PlaySE("Missile");
        }
    }

    public void ReThink()
    {
    _isThink = false;
    Debug.LogWarning("ReThink");
    }
    public void Heal()
    {
        Debug.Log("체력 회복");
        Healdraw.Play();
        draw.Play();
        anim.SetTrigger(DoBigShot);
    
        enemyHealth.currentHealth += enemyHealth.maxHealth;
        if (enemyHealth.currentHealth >= enemyHealth.maxHealth)
        {
            enemyHealth.currentHealth = enemyHealth.maxHealth;
        }
    }

    public void Taunt()
    {
        if (target != null)
        {
            Jump.Play();
            tauntVec = target.transform.position + lookVec;
            nav.SetDestination(tauntVec);
       
            /* Vector3 targetPosition = target.transform.position;
             transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);*/
        }
    
    }

    public void JumpAttack()
    {
        RaycastHit hit;
        if (Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, out hit, raycastDistance, LayerMask.GetMask("Player")))
        {
        
        }
    }
}
