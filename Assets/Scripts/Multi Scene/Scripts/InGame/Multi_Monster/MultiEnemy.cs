using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Data;
using Newtonsoft.Json;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MultiEnemy : MonoBehaviour
{
    public enum EnemyType
    {  
        Green,
        Red,
        Box
    }
    private enum EnemyState
    {
        Idle,
        Chase,
        Die,
    }
    
    private static readonly int IsAttack = Animator.StringToHash("isAttack");
    private static readonly int AniEnemy = Animator.StringToHash("aniEnemy");
    
    public Transform _targetPos; // Player의 Transform 컴포넌트
    private NavMeshAgent _nav; // NavMeshAgent 컴포넌트
    
    private Vector3 _originalPos; //몬스터 원래 위치
    [HideInInspector] public Animator anim; //몬스터 애니메이션

    private const float AttackRadius = 3f; //공격 가능 범위
    private const float DetectionRadius = 7f; //플레이어 감지 범위
    public EnemyType enemyType; //해당 몬스터 타입
    private int _index = -1; //해당 몬스터 인덱스
    private int _damage; //해당 몬스터 데미지

    private EnemyState _currentState;
    private readonly Collider[] _targets = new Collider[5];
    private string _enemyName;
    private string currentSceneName;
    public bool isDead { get; set; }

    public Coroutine AttackCoroutine;
    public Coroutine DetectCoroutine;

    private Transform currentTarget;

    private void Awake()
    {
        _nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }
    private void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
        _originalPos = transform.position;
        _currentState = EnemyState.Idle;
        SetState(currentSceneName);
    }
    
    private void SetState(string sceneName)
    {
        if (sceneName.Equals("Game Scene"))
        {
            string json = "";
            if (enemyType == EnemyType.Box)
            {
                json = "{\"damage\": 100}";
                _enemyName = "Red Monster";
            }
            else if (enemyType == EnemyType.Red)
            {
                json = "{\"damage\": 150}";
                _enemyName = "Red Spider";
            }
            else if (enemyType == EnemyType.Green)
            {
                json = "{\"damage\": 200}";
                _enemyName = "Green Spider";
            }

            EnemyStat enemy = JsonConvert.DeserializeObject<EnemyStat>(json);
            _damage = (int)enemy.damage;
        }
    }

    public void SetIndex()
    {
        _index = MultiScene.Instance.enemyList.IndexOf(gameObject);
        _targetPos = null;
        _nav.SetDestination(_originalPos);
    }
   
    public IEnumerator PlayerDetect()
    {
        if (!MultiScene.Instance.isMasterClient) yield break;
        
        WaitForSeconds wait = new WaitForSeconds(1f);
        EnemyState lastState = EnemyState.Idle; // 이전 상태를 저장하는 변수

        yield return new WaitForSeconds(Random.Range(0f, 2f)); //코루틴 분산

        while (true)
        {
            if (isDead) yield break;
            
            int players = Physics.OverlapSphereNonAlloc(transform.position, DetectionRadius, _targets,
                LayerMask.GetMask("Player"));

            float closestDistance = Mathf.Infinity;

            if (players <= 0) _targetPos = null;

            if (_targetPos == null)
            {
                SetDestination(_targetPos, true);

                if (_nav.remainingDistance <= 0.1f)
                {
                    if (_currentState != EnemyState.Idle)
                    {
                        _currentState = EnemyState.Idle;
                        SetEnemyAnimation(AniEnemy, (int)_currentState);
                        MultiScene.Instance.BroadCastingEnemyAnimation(_index, (int)_currentState);
                    }
                }

                yield return wait;
            }

            for (int i = 0; i < players; i++)
            {
                var target = _targets[i];

                float distance = Vector3.Distance(transform.position, target.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    _targetPos = target.transform;
                }
            }

            if (_targetPos != null) //추적
            {
                if (_currentState != EnemyState.Chase)
                {
                    _currentState = EnemyState.Chase;
                    SetEnemyAnimation(AniEnemy, (int)_currentState);
                    MultiScene.Instance.BroadCastingEnemyAnimation(_index, (int)_currentState);
                }
            }

            SetDestination(_targetPos, true);

            yield return wait;
        }
    }

    public void SetEnemyAnimation(int id, int state = 0, bool isTrigger = false)
    {
        if (isTrigger) anim.SetTrigger(id);
        else anim.SetInteger(id, state);
    }

    public void SetDestination(Transform target, bool isNetwork = false)
    {
        if (_index < 0 || _index > MultiScene.Instance.enemyList.Count) return;

        if (target != currentTarget || (_nav.remainingDistance > 0.15f && target != null))
        {
            _targetPos = target;
            currentTarget = target;

            _nav.SetDestination(target == null ? _originalPos : target.transform.position);

            if (isNetwork)
            {
                MultiScene.Instance.BroadCastingSetEnemyTarget(_targetPos != null ? _targetPos.name : "", _index);
            }
        }
    }

    public void Attack()
    {
        if (!MultiScene.Instance.isMasterClient) return;
        if (!IsAttackable() || isDead || _targetPos==null) return;
            
        SoundManager.instance.PlaySE(_enemyName);
        
        _targetPos.TryGetComponent(out MultiPlayerHealth playerHealth);

        if (playerHealth != null)
        {
            if(playerHealth.CurrentHealth <= 0) return;
            
            playerHealth.TakeDamage(_damage);
            MultiScene.Instance.BroadCastingTakeDamage(_targetPos != null ? _targetPos.name : "", _damage);
        }
    }

    public IEnumerator TryAttack()
    {
        if (!MultiScene.Instance.isMasterClient) yield break;
        
        WaitForSeconds wait = new WaitForSeconds(1.5f);
        
        yield return new WaitForSeconds(Random.Range(0f, 2f)); //코루틴 분산
        
        while (true)
        {
            if(isDead) yield break;
            
            if(IsAttackable())
            {
                yield return wait;
                SetEnemyAnimation(IsAttack, isTrigger: true);
                MultiScene.Instance.BroadCastingEnemyAnimation(_index, IsAttack, true);
            }
            
            yield return wait;
        }
    }

    private bool IsAttackable()
    {
        if(_targetPos == null || isDead) return false;
        
        float distance = Vector3.Distance(transform.position, _targetPos.position);

        return distance <= AttackRadius;
    }
}