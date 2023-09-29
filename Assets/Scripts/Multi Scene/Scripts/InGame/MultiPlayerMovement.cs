using mino;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using Newtonsoft.Json;
using System.IO;
using System;
using UnityEngine.UI;
using System.Timers;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using Data;
using UnityEngine.Serialization;


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
    //public SpacebarCooldownUI cooldownUI;
  //  public TMP_Text coolText;
  //  public GameObject SpaceUI;
   // public UnityEngine.UI.Image fill;

    private bool _isCoolingDown;
    private float _cooldownEndTime;
    private readonly float _cooldownTime = 10f;
    private float _currentCooldown = 10f;
    private float _maxCooldown = 10f;
    void Start()
    {
        ChangedState(PlayerState.Idle);
        ani = GetComponent<AniSetting>();
        _weaponController = GetComponent<MultiWeaponController>();
        MultiScene.Instance.BroadCastingAnimation((int)PlayerState.Idle);
        //    SpaceUI.SetActive(false);//스페이스바 UI 비활성화
    }
    private void Update()
    {
        if(!MultiScene.Instance.currentUser.Equals(gameObject.name)) return;
        
        if (Input.GetMouseButtonDown(1)) // 오른쪽 클릭
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            LayerMask layerMask = ~LayerMask.GetMask("Player");

            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, layerMask))
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
        
        //만약 플레이어가 목적지에 도착하였을때! 다시 애니메이션을 기본상태로 되돌림 , 만든이:방민호
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
            //SpaceUI.SetActive(true);
            _isCoolingDown = true;
        
            _cooldownEndTime = Time.time + _cooldownTime;                           
        
        }
        if (_isCoolingDown)
        {
            float remainingTime = Mathf.Max(0, _cooldownEndTime - Time.time);
        
            if (remainingTime > 0)
            {
                //SpaceBarUI();//시간이 남아있는동안 실행시킬거임
                //coolText.text = Mathf.CeilToInt(remainingTime).ToString();
            }
            else
            {
                //SpaceUI.SetActive(false);
                _isCoolingDown = false;
                //coolText.text = "";
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
    
    public void Attack()
    {
        if (_weaponController.currentTarget != null)
        {
            _weaponController.equippedWeapon.Attack(_weaponController.currentTarget);
            
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
            case PlayerState.BowAttackIdle:
                PlayerStateBowAttackIdle();
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


    // #region 스킬 쿨타임 전용 UI제작 Methods
    // public void SpaceBarUI()
    // {
    //     SetCurrentCooldown(currentCooldown - Time.deltaTime);
    //
    //     if (currentCooldown < 0f)
    //         currentCooldown = MaxCooldown;
    // }
    // private void UpdateFillAmount()
    // {
    //     fill.fillAmount = currentCooldown / MaxCooldown;
    // }
    // public void SetMaxCooldown(in float value)
    // {
    //     MaxCooldown = value;
    //     UpdateFillAmount();
    // }
    //
    // public void SetCurrentCooldown(in float value)
    // {
    //     currentCooldown = value;
    //     UpdateFillAmount();
    // }
    // #endregion-제작자 방민호


}
