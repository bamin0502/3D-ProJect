using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Data;
using Newtonsoft.Json;
using UnityEngine.UIElements;

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

public int missileDmg;
public int meleeDmg;

public EnemyHealthBar enemyHealthBar;
public EnemyHealth enemyHealth;
public int healAmount;
public float maxHealth;
public float currentHealth;

public GameObject target;

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
void SelectTarget()
{
    if(!MultiScene.Instance.isMasterClient) return;
    int randomIndex = MultiScene.Instance.GetRandomInt(hitCount - 1);
    target = _targets[randomIndex].gameObject;
    MultiScene.Instance.BroadCastingTargetSet(target.name);
}
public void Update()
{
    if (target != null)
    {
        transform.LookAt(target.transform.position);
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
    switch (ranAction)
    {
        default:
            LaunchMissile();
            MultiScene.Instance.BroadCastingSkill(0);
            anim.SetTrigger(DoShot);
            MultiScene.Instance.BroadCastingBossAnimation(DoShot,true); ;
            break;
        
        /*case 2:
            break;
        case 3:
            Debug.LogWarning("Heal");
            //StartCoroutine(Heal());
            break;
        case 4:
            Debug.LogWarning("Taunt");
            //StartCoroutine(Taunt());
            break;
        case 5:
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
}
IEnumerator Heal()
{
    Debug.Log("체력 회복");
    Healdraw.Play();
    Debug.Log("Healdraw");
    draw.Play();
    Debug.Log("Healdraw.play");
    anim.SetTrigger(DoBigShot);
    Debug.Log("anim Play");
    MultiScene.Instance.BroadCastingBossAnimation(DoBigShot,true);

    enemyHealth.currentHealth += enemyHealth.maxHealth;
    if (enemyHealth.currentHealth >= enemyHealth.maxHealth)
    {
        enemyHealth.currentHealth = enemyHealth.maxHealth;
    }
    
    yield return new WaitForSeconds(5f);
    Think();

}

IEnumerator Taunt()
{
    yield return new WaitForSeconds(5f);
    Think();
}
}
