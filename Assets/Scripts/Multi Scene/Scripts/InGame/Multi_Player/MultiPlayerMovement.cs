using mino;
using UnityEngine;
using UnityEngine.AI;
using System;
using Cinemachine;
using TMPro;
using UnityEngine.UI;

public class MultiPlayerMovement : MonoBehaviour
{
    //만든이: 임성훈
    private const float RotAnglePerSecond = 360f;
    private Vector3 _curTargetPos;
    
    private MultiWeaponController _weaponController;
    public NavMeshAgent navAgent;
    public Camera _camera;
    
    //만든이: 방민호
    public PlayerState currentState = PlayerState.Idle;
    public AniSetting ani;
    
    public Action OnDead; //죽었을때 호출할 이벤트
    public TMP_Text coolText;
    public GameObject spaceUI;
    public Image fill;

    private bool _isCoolingDown;
    private float _cooldownEndTime;
    private readonly float _cooldownTime = 10f;
    private float _currentCooldown = 10f;
    private float _maxCooldown = 10f;

    private bool canMove = true;
    void Start()
    {
        ChangedState(PlayerState.Idle);
        ani = GetComponent<AniSetting>();
        _weaponController = GetComponent<MultiWeaponController>();
        MultiScene.Instance.BroadCastingAnimation((int)PlayerState.Idle);

        if (spaceUI != null)
        {
            spaceUI.SetActive(false);//스페이스바 UI 비활성화    
        }
        
    }

    public void StopMovement()
    {
        canMove = false;
        NavStop();
    }
    public void ResumeMovement()
    {
        canMove = true;
        NavResume();
    }
    private void Update()
    {
        if(!MultiScene.Instance.currentUser.Equals(gameObject.name)) return;
        if (!canMove)
            return;
        // 오른쪽 마우스 클릭을 확인합니다.
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            
            LayerMask playerMask = LayerMask.GetMask("Player");
            LayerMask enemyMask = LayerMask.GetMask("Enemy");
            LayerMask wallMask= LayerMask.GetMask("Wall");
            LayerMask combinedMask = playerMask | enemyMask | wallMask;
            LayerMask layerMask = ~combinedMask;
            
            

            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, enemyMask))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    var isBoss = hit.transform.GetComponent<MultiBoss>();

                    if (isBoss != null)
                    {
                        _weaponController.SetTarget(-1, true);
                        MultiScene.Instance.BroadCastingMovement(hit.transform.position, -1, true);
                    }
                    else
                    {
                        int enemy = MultiScene.Instance.enemyList.IndexOf(hit.transform.gameObject);
                        
                        _weaponController.SetTarget(enemy);
                        MultiScene.Instance.BroadCastingMovement(hit.transform.position, enemy);
                    }
                }
            }
            
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                _weaponController.ClearTarget();

                if (!_weaponController.isAttack)
                {
                    navAgent.SetDestination(hit.point);
                    ChangedState(PlayerState.RunForward);

                    MultiScene.Instance.BroadCastingMovement(hit.point);
                    MultiScene.Instance.BroadCastingAnimation((int)PlayerState.RunForward);
                }
                else
                {
                    ChangedState(PlayerState.HammerAttackIdle);
                    MultiScene.Instance.BroadCastingAnimation((int)PlayerState.HammerAttackIdle);
                }
            }
        }
        
        if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance && !navAgent.hasPath)
        {
            if (currentState == PlayerState.RunForward)
            {
                ChangedState(PlayerState.Idle);
                MultiScene.Instance.BroadCastingAnimation((int)PlayerState.Idle);
            }
        }
        
        if (!_isCoolingDown && Input.GetKeyDown(KeyCode.Space))
        {
            ChangedState(PlayerState.SpaceMove);
            MultiScene.Instance.BroadCastingAnimation((int)PlayerState.SpaceMove);
            spaceUI.SetActive(true);
            _isCoolingDown = true;
            _cooldownEndTime = Time.time + _cooldownTime;                           
        }
        if (_isCoolingDown)
        {
            float remainingTime = Mathf.Max(0, _cooldownEndTime - Time.time);
        
            if (remainingTime > 0)
            {
                SpaceBarUI();//시간이 남아있는동안 실행시킬거임
                coolText.text = Mathf.CeilToInt(remainingTime).ToString();
            }
            else
            {
                spaceUI.SetActive(false);
                _isCoolingDown = false;
                coolText.text = "";
            }
        }
    }

    // #region 플레이어 회전관련
    public void TurnToDestination()
    {
        //회전
        Quaternion lookRotation = Quaternion.LookRotation(_curTargetPos - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.deltaTime * RotAnglePerSecond);
    }
    // #endregion

    public void Dead()
    {
        Destroy(gameObject);
    }
    
    public void Attack(int isSkill)
    {
        if (_weaponController.currentTarget != null)
        {
            _weaponController.equippedWeapon.Attack(_weaponController.currentTarget, isSkill);
            
            if (_weaponController.currentTarget.TryGetComponent(out Enemy enemy))
            {
                if (enemy.isDead) _weaponController.currentTarget = null;
            }
        
            else if (_weaponController.currentTarget.TryGetComponent(out Boss boss))
            {
                if (boss.isDead) _weaponController.currentTarget = null;
            }
        }
    }

    #region 플레이어 애니메이션 관련 -제작자 방민호
    public void UpdateState()
    {
        switch (currentState) 
        {
            case PlayerState.Idle:
                PlayerStateIdle();
                break;
            case PlayerState.RunForward:
                PlayerStateRunForward();
                break;
            case PlayerState.SpaceMove:
                PlayerStateSpaceMoveIdle();
                break;
            case PlayerState.Dead:
                PlayerStateDead();
                break;
            case PlayerState.GetHit:
                PlayerStateGetHit();
                break;
            default:
                break;
        }

    }
    public void PlayerStateGetHit()
    {
        
    }
    void PlayerStateIdle()
    {

    }
    void PlayerStateRunForward()
    {
       TurnToDestination();
        
    }
    void PlayerStateBowAttackIdle()
    {
        TurnToDestination();
        
    }
    void PlayerStateSpaceMoveIdle()
    {
       TurnToDestination();
       _isCoolingDown = false;
        
    }
    void PlayerStateDead()
    {
       OnDead += Dead;      
    }
    public void ChangedState(PlayerState newState)
    {
        if (currentState == newState) return;
        ani.ChangeAnimation(newState);
        currentState = newState;
    }
    public void SetAnimationTrigger(int aniNum)
    {
        ani.SetTrigger(aniNum);
    }
    
    #endregion
    
    public void SpaceBarUI()
    {
        SetCurrentCooldown(_currentCooldown - Time.deltaTime);
    
        if (_currentCooldown < 0f)
            _currentCooldown = _maxCooldown;
    }
    private void UpdateFillAmount()
    {
        fill.fillAmount = _currentCooldown / _maxCooldown;
    }
    public void SetMaxCooldown(in float value)
    {
        _maxCooldown = value;
        UpdateFillAmount();
    }
    
    public void SetCurrentCooldown(in float value)
    {
        _currentCooldown = value;
        UpdateFillAmount();
    }

    public void NavStop()
    {
        navAgent.isStopped = true;
    }
    
    public void NavResume()
    {
        navAgent.isStopped = false;
    }
}
