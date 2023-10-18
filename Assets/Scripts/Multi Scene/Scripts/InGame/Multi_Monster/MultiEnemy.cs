using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Data;
using Newtonsoft.Json;
using UnityEngine.Serialization;
using UnityEngine.UI;

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
    
    private Transform _targetPos; // Player의 Transform 컴포넌트
    private NavMeshAgent _nav; // NavMeshAgent 컴포넌트
    
    private Vector3 _originalPos; //몬스터 원래 위치
    [HideInInspector] public Animator anim; //몬스터 애니메이션

    private const float AttackRadius = 2f; //공격 가능 범위
    private const float DetectionRadius = 7f; //플레이어 감지 범위
    public EnemyType enemyType; //해당 몬스터 타입
    private int _index; //해당 몬스터 인덱스
    private int _damage; //해당 몬스터 데미지

    private EnemyState _currentState;
    private readonly Collider[] _targets = new Collider[5];
    private string _enemyName;

    private void Awake()
    {
        _nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }
    private void Start()
    {
        _currentState = EnemyState.Idle;
        anim.SetInteger(AniEnemy, (int)_currentState);
        MultiScene.Instance.BroadCastingEnemyAnimation(_index, (int)_currentState);
        _index = MultiScene.Instance.enemyList.IndexOf(gameObject);
        _originalPos = transform.position;
        SetState();
    }

    private void SetState()
    {
        string json = "";

        if (enemyType == EnemyType.Box)
        {
            json = "{\"damage\": 30}";
            _enemyName = "Red Monster";
        }
        else if (enemyType == EnemyType.Red)
        {
            json = "{\"damage\": 10}";
            _enemyName = "Red Spider";
        }
        else if (enemyType == EnemyType.Green)
        {
            json = "{\"damage\": 20}";
            _enemyName = "Green Spider";
        }

        EnemyStat enemy = JsonConvert.DeserializeObject<EnemyStat>(json);
        _damage = (int)enemy.damage;
    }
   
    public IEnumerator PlayerDetect()
    {
        WaitForSeconds wait = new WaitForSeconds(0.5f);
        
        while (true)
        {
            int players = Physics.OverlapSphereNonAlloc(transform.position, DetectionRadius, _targets, LayerMask.GetMask("Player"));

            float closestDistance = Mathf.Infinity;

            if (players <= 0)
            {
                _targetPos = null;
                _nav.SetDestination(_originalPos);
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
                    _nav.SetDestination(_targetPos.position);
                }
            }

            if (_targetPos != null) //추적
            {
                if(_currentState == EnemyState.Chase) yield return wait;
                _currentState = EnemyState.Chase;
                anim.SetInteger(AniEnemy, (int)_currentState);
                MultiScene.Instance.BroadCastingEnemyAnimation(_index, (int)_currentState);
            }
            else //멈춤
            {
                if (_currentState == EnemyState.Idle) yield return wait;
                _currentState = EnemyState.Idle;
                anim.SetInteger(AniEnemy, (int)_currentState);
                MultiScene.Instance.BroadCastingEnemyAnimation(_index, (int)_currentState);
            }

            yield return wait;
        }
    }

    public void Attack()
    {
        if (!IsAttackable()) return;
            
        SoundManager.instance.PlaySE(_enemyName);
        
        bool isPlayer = _targetPos.TryGetComponent(out MultiPlayerHealth playerHealth);
        
        if (isPlayer)
        {
            playerHealth.TakeDamage(_damage);
        }
    }

    public IEnumerator TryAttack()
    {
        WaitForSeconds wait = new WaitForSeconds(1);
        
        while (true)
        {
            if(IsAttackable())
            {
                anim.SetTrigger(IsAttack);
                MultiScene.Instance.BroadCastingEnemyAnimation(_index, IsAttack, true);
            }
            
            yield return wait;
        }
    }

    private bool IsAttackable()
    {
        if(_targetPos == null) return false;
        
        float distance = Vector3.Distance(transform.position, _targetPos.position);

        return distance <= AttackRadius;
    }
}


