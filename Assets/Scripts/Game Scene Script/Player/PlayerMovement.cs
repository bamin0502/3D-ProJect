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

class PlayerStat 
{
    public float damage=0;
    public float Health=0;
}
class EnemyStat 
{
    public float damage = 0;
    public float Health = 0;
}


public class PlayerMovement : MonoBehaviour
{
    //임성훈   
    private NavMeshAgent _navAgent;
    private Camera _camera;
    private float rotAnglePerSecond = 360f;
    private Vector3 curTargetPos;
    //방민호
    public PlayerState currentState = PlayerState.Idle;
    private AniSetting ani;
    public Action OnDead;//죽었을때 호출할 이벤트
    bool isCoolingDown = false;
    float cooldownEndTime = 0f;
    float cooldownTime = 10f;
    //public SpacebarCooldownUI cooldownUI;
    public TMP_Text coolText;

    public UnityEngine.UI.Image fill;
    private float MaxCooldown = 10f;
    private float currentCooldown = 10f;
    void Start()
    {
        Managers mag = Managers.GetInstance();//방민호
        _navAgent = GetComponent<NavMeshAgent>();
        _camera = Camera.main;
        ani=GetComponent<AniSetting>();

        ChangedState(PlayerState.Idle);

        var Playerstat = new PlayerStat //플레이어 설정
        {
            damage = 20,
            Health = 200
        };
        var Enemystat1 = new EnemyStat //몬스터1 설정
        {
            damage=20,
            Health=50
        };
        var Enemystat2 = new EnemyStat //몬스터2 설정
        {
            damage=10,
            Health=70
        };
        var Enemystat3 = new EnemyStat //몬스터3 설정
        {
            damage=30,
            Health=30
        };
        File.WriteAllText(Application.dataPath + "/PlayerStat.json", JsonUtility.ToJson(Playerstat));
        Debug.Log(Application.dataPath + "/PlayerStat.json");
        File.WriteAllText(Application.dataPath + "/EnemyStat.json", JsonUtility.ToJson(Enemystat1));
        Debug.Log(Application.dataPath + "/EnemyStat.json");
        File.WriteAllText(Application.dataPath + "/EnemyStat.json", JsonUtility.ToJson(Enemystat2));
        Debug.Log(Application.dataPath + "/EnemyStat.json");
        File.WriteAllText(Application.dataPath + "/EnemyStat.json", JsonUtility.ToJson(Enemystat2));
        Debug.Log(Application.dataPath + "/EnemyStat.json");
        //로그작동되는지 확인(확인결과 이상무)
        //TakeDamage();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) // 오른쪽 클릭
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit))
            {
                // 이동할 위치로 플레이어를 이동
                _navAgent.SetDestination(hit.point);
                ChangedState(PlayerState.RunForward);
            }

        }
        //만약 플레이어가 목적지에 도착하였을때! 다시 애니메이션을 기본상태로 되돌림
        if (!_navAgent.pathPending && _navAgent.remainingDistance <= _navAgent.stoppingDistance && !_navAgent.hasPath)
        {
            ChangedState(PlayerState.Idle);
        }
        if (!isCoolingDown && Input.GetKeyDown(KeyCode.Space))
        {
            ChangedState(PlayerState.SpaceMove);
            
            isCoolingDown = true;
            cooldownEndTime = Time.time + cooldownTime;                           

        }
        if (isCoolingDown)
        {

            float remainingTime = Mathf.Max(0, cooldownEndTime - Time.time);

            if (remainingTime > 0)
            {
                SpaceBarUI();
                coolText.text = "쿨타임까지 " + Mathf.CeilToInt(remainingTime).ToString() + "초 남았습니다."; ;
            }
            else
            {
                isCoolingDown = false;
                coolText.text = "";
            }
        }
        //몬스터에 맞아서 GetHit 애니메이션이 실행되면 TakeDamage를 호출시킨다.
        if (currentState == PlayerState.GetHit)
        {
            TakeDamage();
        }
    }
    
    //내용추가 방민호 Json화
    public void TakeDamage()
    {
        //PlayerStat.json으로 저장되어 있는 파일의 값을 읽어 온다.
        string LoadPlayerstat = File.ReadAllText(Application.dataPath + "/PlayerStat.json");
        Debug.Log("ReadAllText :" + LoadPlayerstat);
        string LoadEnemyStat = File.ReadAllText(Application.dataPath + "/EnemyStat.json");
        Debug.Log("ReadAllText :" + LoadEnemyStat);
        //FromJson으로 값을 가져오고 log로 데이터를 역직렬화를 시킨다.
        PlayerStat data = JsonUtility.FromJson<PlayerStat>(LoadPlayerstat);
        EnemyStat endata = JsonUtility.FromJson<EnemyStat>(LoadEnemyStat);
        string log = string.Format("data {0},{1}", endata.damage, data.Health);
        Debug.Log(log);
        //현재 체력 - 현재 데미지
        data.Health -= endata.damage;

        //다시 직렬화를 시켜준다. 다만 이건 플레이어의 공격력과 체력 두개의 값으로
        //아마 몬스터가 되면 몬스터의 공격력하고 플레이어의 체력을 가져온뒤에 넣어야할거같다
        // 그래서 아마 플레이어의 TakeDamage는 플레이어의 체력- 몬스터 공격력
        // 몬스터의 TakeDamage는 몬스터 체력- 플레이어의 공격력이 되도록
        string TakeDamage = JsonUtility.ToJson(data);
        File.WriteAllText(Application.dataPath + "/PlayerStat.json", TakeDamage);
        string TakeEnemy = JsonUtility.ToJson(endata);
        File.WriteAllText(Application.dataPath + "EnemyStat.json", TakeEnemy);
        //플레이어의 체력이 0보다 크거나 같으면 캐릭터의 애니메이션을 Dead로 만들고 OnDead Delegate 호출
        if(data.Health <= 0)
        {
            ChangedState(PlayerState.Dead);
            OnDead.Invoke();//아직 연결안함
        }
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
    void PlayerStateGetHit()
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
}
