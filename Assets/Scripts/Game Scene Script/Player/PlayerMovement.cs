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
using Sirenix.OdinInspector;


public class PlayerMovement : SerializedMonoBehaviour
{
    //만든이: 임성훈   
    public WeaponController weaponController;
    private NavMeshAgent _navAgent;
    private Camera _camera;
    private float rotAnglePerSecond = 360f;
    private Vector3 curTargetPos;
    //만든이: 방민호
    public PlayerState currentState = PlayerState.Idle;
    private AniSetting ani;
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
    private bool isAttacking = false;

    void Start()
    {
        //만든이 : 임성훈
        _navAgent = GetComponent<NavMeshAgent>();
        _camera = Camera.main;
        //만든이 : 방민호
        Managers mag = Managers.GetInstance();
        ani = GetComponent<AniSetting>();
        SpaceUI.SetActive(false);//스페이스바 UI 비활성화
        ChangedState(PlayerState.Idle);//플레이어 기본상태를 Idle로 지정

    }

    IEnumerator AttackRoutine(WeaponType weaponType, PlayerState attackState, PlayerState idleState, float attackInterval)
    {
        while (isAttacking)
        {
            // Attack animation 시작
            ChangedState(idleState);
            yield return new WaitForSeconds(0.1f);
            float curAnimationTime = ani.ani.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(curAnimationTime);
            ChangedState(attackState);
            yield return new WaitForSeconds(attackInterval);
        }

    }

    void Attack(){
        if(weaponController.equippedWeapon != null){
            weaponController.equippedWeapon.Attack(weaponController.currentTarget);
        }
    }

    private void Update()
    {

        //만든이 : 임성훈 
        if (Input.GetMouseButtonDown(1)) // 오른쪽 클릭
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit))
            {
                // 이동할 위치로 플레이어를 이동
                _navAgent.SetDestination(hit.point);

                float distance = Vector3.Distance(transform.position, hit.point);

                if (weaponController.currentTarget == null)
                {
                    StopAllCoroutines();
                    ChangedState(PlayerState.RunForward);
                    isAttacking = false;
                }

                if (weaponController.currentTarget != null && distance <= weaponController.equippedWeapon.range)
                {
                    isAttacking = true;

                    float interval = weaponController.equippedWeapon.attackInterval;
                    
                    switch (weaponController.equippedWeapon.weaponType)
                    {
                        case WeaponType.Gun:
                            //총
                            break;
                        case WeaponType.Bow:
                            StartCoroutine(AttackRoutine(WeaponType.Bow, PlayerState.BowAttackIdle, PlayerState.Idle, interval));
                            break; //활
                        case WeaponType.OneHanded:
                            StartCoroutine(AttackRoutine(WeaponType.OneHanded, PlayerState.ShortAttack, PlayerState.Idle, interval));
                            break; //한손
                        case WeaponType.TwoHanded:
                            StartCoroutine(AttackRoutine(WeaponType.TwoHanded, PlayerState.HammerAttackIdle, PlayerState.Idle, interval));
                            break; //양손
                        default:
                            break; //없음
                    }
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


    }

    //내용추가 만든이 : 방민호 Json화
    public static void TakeDamage()
    {

        DataManager.Inst.SetPlayerAttack();

    }
    void ChangedState(PlayerState newState)
    {
        if (currentState == newState)
            return;
        ani.ChangeAnimation(newState);
        currentState = newState;

    }
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
        TakeDamage();
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
    public void TurnToDestination()
    {
        //회전
        Quaternion lookRotation = Quaternion.LookRotation(curTargetPos - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.deltaTime * rotAnglePerSecond);
    }
    public void Dead()
    {
        Destroy(gameObject);
    }

    ///스킬 쿨타임 전용 UI제작 Methods///
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
    ///-------------------------///
}
