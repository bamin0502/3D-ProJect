using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Data;
using Newtonsoft.Json;
using DamageNumbersPro;
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

    private bool isBossDead = false;
    private bool deathAnimationPlayed= false;
    private int isCalled = 0;
    
    //점프 이동
    public float moveSpeed = 5.0f;
    private Vector3 lookVec;
    private Vector3 tauntVec;

    //점프 공격
    public GameObject jumpObj;
    public int meleeDmg;
    public int missileDmg;
    
    public EnemyHealthBar enemyHealthBar;
    public EnemyHealth enemyHealth;
    public int healAmount;
    public DamageNumber damageNumber;
    public Transform damageNumberPos;

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
        string json = "{\"Heal\": 1000}";
        EnemyStat enemyStat1 = JsonConvert.DeserializeObject<EnemyStat>(json);
        healAmount = (int)enemyStat1.Heal;
        jumpObj.gameObject.SetActive(false);
        StopCoroutine(PlayerDetect());
        StopCoroutine(ChangeTarget());
        StopCoroutine(StartThink());
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
        if(enemyHealth.currentHealth <= 0&& !isBossDead)
        {
            isBossDead = true;
            StartCoroutine(DeleteSelf());
            StopCoroutine(StartThink());
        }
    }
    public void Stop()
    {
        StopCoroutine(PlayerDetect());
        StopCoroutine(ChangeTarget());
        StopCoroutine(StartThink());
    }
    public IEnumerator PlayerDetect()
    {
        if (!MultiScene.Instance.isMasterClient) yield break;
        WaitForSeconds wait = new WaitForSeconds(1.5f);
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
        WaitForSeconds wait = new WaitForSeconds(1f);
        while(true)
        {
            jumpObj.gameObject.SetActive(false);
            if(target == null || _isThink) yield return wait;
            Think();
            yield return wait;
        }

    }

    IEnumerator DeleteSelf()
    {
        anim.SetTrigger(Dodie);
        MultiScene.Instance.BroadCastingBossAnimation(Dodie, true);
        yield return new WaitForSeconds(8.0f);
        DestroyBoss();
        MultiScene.Instance.BroadCastingSkill(3);
    }
    public void SelectTarget()
    {
        int randomIndex = MultiScene.Instance.GetRandomInt(hitCount - 1);
        target = _targets[randomIndex].gameObject;
        targetPos = target.gameObject.transform;
        MultiScene.Instance.BroadCastingTargetSet(target.name);
    }


    public void LaunchMissile()
    {
        MissileShot(missilePortA);
        MissileShot(missilePortB);
    }



    public void Think()
    {
        if (!MultiScene.Instance.isMasterClient) return;
        int ranAction = MultiScene.Instance.GetRandomInt(4);
        if(target == null) return;
        _isThink = true;
        switch (ranAction)
        {
            case 0:
                LaunchMissile();
                MultiScene.Instance.BroadCastingSkill(0);
                anim.SetTrigger(DoShot);
                MultiScene.Instance.BroadCastingBossAnimation(DoShot,true); 
                break;
            case 1:
                Taunt();
                MultiScene.Instance.BroadCastingSkill(2);
                anim.SetTrigger(DoTaunt);
                Invoke(nameof(setJumpObj),1.7f);
                MultiScene.Instance.BroadCastingBossAnimation(DoTaunt,true); 
                break;
            case 2:
                Heal();
                MultiScene.Instance.BroadCastingSkill(1);
                anim.SetTrigger(DoBigShot);
                MultiScene.Instance.BroadCastingBossAnimation(DoBigShot,true); 
                break;
            case 3:
                break;

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
        Healdraw.Play();
        draw.Play();
        
        enemyHealth.currentHealth = Math.Min(enemyHealth.currentHealth + healAmount, enemyHealth.maxHealth);
        damageNumber.Spawn(damageNumberPos.transform.position, healAmount);
        enemyHealthBar.UpdateBossHealth();
    }
    public void DestroyBoss()
    {
        Destroy(gameObject);
    }
    public void Taunt()
    {
        if (target != null)
        {
            Jump.Play();
            tauntVec = target.transform.position + lookVec;
            nav.SetDestination(tauntVec);
        }
    }
    public void setJumpObj()
    {
        jumpObj.gameObject.SetActive(true);
    }

}
