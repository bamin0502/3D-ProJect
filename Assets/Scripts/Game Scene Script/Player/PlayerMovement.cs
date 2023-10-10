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
using Image = UnityEngine.UIElements.Image;


public class PlayerMovement : MonoBehaviour
{
    //만든이: 임성훈
    [SerializeField] private WeaponController weaponController;
    private NavMeshAgent _navAgent;
    private Camera _camera;
    private float rotAnglePerSecond = 360f;
    private Vector3 curTargetPos;
    //만든이: 방민호
    public PlayerState currentState = PlayerState.Idle;
    public AniSetting ani;
    public Action OnDead;//죽었을때 호출할 이벤트
    bool isCoolingDown = false;
    float cooldownEndTime = 0f;
    float cooldownTime = 10f;
    //public SpacebarCooldownUI cooldownUI;
    public TMP_Text coolText;
    public GameObject SpaceUI;
    public UnityEngine.UI.Image fill;
    private float MaxCooldown = 10f;
    private float currentCooldown = 10f;

    void Start()
    {
        //만든이 : 임성훈
        _navAgent = GetComponent<NavMeshAgent>();
        _camera = Camera.main;
        //만든이 : 방민호
        coolText= GameObject.Find("Spacebar Image").transform.Find("Spacebar Text").GetComponent<TMP_Text>();
        SpaceUI= GameObject.Find("Spacebar Image");
        fill= GameObject.Find("Spacebar Image").GetComponent<UnityEngine.UI.Image>();
       
        ani =GetComponent<AniSetting>();
        SpaceUI.SetActive(false);//스페이스바 UI 비활성화
        ChangedState(PlayerState.Idle);//플레이어 기본상태를 Idle로 지정
        
    }
    private void Update()
    {
        #region 플레이어 이동관련
        //만든이 : 임성훈 
        if (Input.GetMouseButtonDown(1)) // 오른쪽 클릭
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            LayerMask layerMask = ~LayerMask.GetMask("Player");

            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, layerMask))
            {
                if (!weaponController.isAttack)
                {
                    _navAgent.SetDestination(hit.point);
                    ChangedState(PlayerState.RunForward);
                }
                else
                {
                    ChangedState(PlayerState.HammerAttackIdle);
                }
                
            }
        }
        //만약 플레이어가 목적지에 도착하였을때! 다시 애니메이션을 기본상태로 되돌림 , 만든이:방민호
        if (!_navAgent.pathPending && _navAgent.remainingDistance <= _navAgent.stoppingDistance && !_navAgent.hasPath)
        {
            ChangedState(PlayerState.Idle);
        }
        if (!isCoolingDown && Input.GetKeyDown(KeyCode.Space))
        {
            ChangedState(PlayerState.SpaceMove);
            SpaceUI.SetActive(true);
            isCoolingDown = true;

            cooldownEndTime = Time.time + cooldownTime;                           

        }
        if (isCoolingDown)
        {
            float remainingTime = Mathf.Max(0, cooldownEndTime - Time.time);

            if (remainingTime > 0)
            {
                SpaceBarUI();//시간이 남아있는동안 실행시킬거임
                coolText.text = Mathf.CeilToInt(remainingTime).ToString();
            }
            else
            {
                SpaceUI.SetActive(false);
                isCoolingDown = false;
                coolText.text = "";
            }
        }
        #endregion

    }

    #region 플레이어 회전관련
    public void TurnToDestination()
    {
        //회전
        Quaternion lookRotation = Quaternion.LookRotation(curTargetPos - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.deltaTime * rotAnglePerSecond);
    }
    #endregion

    public void Dead()
    {
        Destroy(gameObject);
    }
    
    public void Attack(int isSkill)
    {
        if (weaponController.currentTarget != null)
        {
            weaponController.equippedWeapon.Attack(weaponController.currentTarget, isSkill);
            
            if (weaponController.currentTarget.TryGetComponent(out Enemy enemy))
            {
                if (enemy.isDead) weaponController.currentTarget = null;
            }

            else if (weaponController.currentTarget.TryGetComponent(out Boss boss))
            {
                if (boss.isDead) weaponController.currentTarget = null;
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
        isCoolingDown = false;
        
    }
    void PlayerStateDead()
    {
        OnDead += Dead;      
    }
    public void ChangedState(PlayerState newState)
    {
        if (currentState == newState)
            return;
        ani.ChangeAnimation(newState);
        currentState = newState;

    }
    #endregion


    #region 스킬 쿨타임 전용 UI제작 Methods
    public void SpaceBarUI()
    {
        SetCurrentCooldown(currentCooldown - Time.deltaTime);

        if (currentCooldown < 0f)
            currentCooldown = MaxCooldown;
    }
    private void UpdateFillAmount()
    {
        fill.fillAmount = currentCooldown / MaxCooldown;
    }
    public void SetMaxCooldown(in float value)
    {
        MaxCooldown = value;
        UpdateFillAmount();
    }
    
    public void SetCurrentCooldown(in float value)
    {
        currentCooldown = value;
        UpdateFillAmount();
    }
    #endregion-제작자 방민호


}
