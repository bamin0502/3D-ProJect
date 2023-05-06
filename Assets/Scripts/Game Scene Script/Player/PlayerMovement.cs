using mino;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using Newtonsoft.Json;
using System.IO;
class PlayerStat 
{
    public float damage=0;
    public float Health=0;
}

public class PlayerMovement : MonoBehaviour
{
    //임성훈   
    private NavMeshAgent _navAgent;
    private Camera _camera;
   //방민호
    private float rotAnglePerSecond = 360f;
    private Vector3 curTargetPos;
    public PlayerState currentState = PlayerState.Idle;
    private AniSetting ani;
    void Start()
    {
        Managers mag = Managers.GetInstance();//방민호
        _navAgent = GetComponent<NavMeshAgent>();
        _camera = Camera.main;
        ani=GetComponent<AniSetting>();

        ChangedState(PlayerState.Idle);


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
        
        //만약 플레이어가 목적지에 도착하였을때! 다시 애니메이션을 기본상태로 되돌림
        if (!_navAgent.pathPending && _navAgent.remainingDistance <= _navAgent.stoppingDistance && !_navAgent.hasPath)
        {
            ChangedState(PlayerState.Idle);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangedState(PlayerState.SpaceMove);
        }
        else if (Input.GetMouseButtonDown(0) && CompareTag("Enemy"))
        {
            PlayerStateBowAttackIdle();
        }
        }
    }

    //내용추가 방민호 Json화
    public void TakeDamage()
    {
        var Playerstat = new PlayerStat
        {
            damage = 10,
            Health = 100
        };
        File.WriteAllText(Application.dataPath + "/PlayerStat.json", JsonUtility.ToJson(Playerstat));
        Debug.Log(Application.dataPath + "/PlayerStat.json");

        string LoadPlayerstat = File.ReadAllText(Application.dataPath + "/PlayerStat.json");
        Debug.Log("ReadAllText :" + LoadPlayerstat);

        PlayerStat data = JsonUtility.FromJson<PlayerStat>(LoadPlayerstat);
        string log = string.Format("data {0},{1}", data.damage, data.Health);
        Debug.Log(log);

    }
    void ChangedState(PlayerState newState)
    {
        if (currentState == newState)
            return;
        ani.ChangeAnimation(newState);
        currentState = newState;
                
    }
    void UpdateState()
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
    }
    void PlayerStateDead()
    {


    }
    public void TurnToDestination()
    {
        //회전
        Quaternion lookRotation = Quaternion.LookRotation(curTargetPos - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.deltaTime * rotAnglePerSecond);
    }

}
