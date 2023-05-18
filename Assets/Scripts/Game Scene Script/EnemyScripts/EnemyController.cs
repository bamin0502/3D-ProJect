using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCon : MonoBehaviour
{
    public float detectionRadius = 10f; // Player를 감지할 범위
    public float returnSpeed = 5f; // 본래 자리로 돌아가는 속도

    private Transform player; // Player의 Transform 컴포넌트
    private Vector3 originalPosition; // 본래 자리의 위치
    private bool isReturning = false; // 본래 자리로 돌아가는 중인지 여부
    Animator anim;
 

    private void Start()

    {
        
        player = GameObject.FindGameObjectWithTag("Player").transform; // Player를 찾아서 Transform을 가져옴
        originalPosition = transform.position; // 본래 자리의 위치를 저장
    }

    private void Update()
    {
    }
    public void radar()
    {
        // Player와 Enemy Object 사이의 거리 계산
        float distance = Vector3.Distance(transform.position, player.position);

        // 일정 거리 내에 Player가 있을 경우 추적
        if (distance <= detectionRadius)
        {
            // 본래 자리로 돌아가는 중이 아니라면 추적 시작
            if (!isReturning)
            {
                // Enemy Object의 이동 방향을 Player의 위치로 설정
                Vector3 direction = (player.position - transform.position).normalized;
                transform.position += direction * Time.deltaTime * returnSpeed;
                
            }
        }
        else if (isReturning) // 추적 중이 아니고 본래 자리로 돌아가는 중인 경우
        {
            // Enemy Object의 이동 방향을 본래 자리로 설정
            Vector3 direction = (originalPosition - transform.position).normalized;
            transform.position += direction * Time.deltaTime * returnSpeed;

            // 본래 자리에 도달하면 본래 자리로 돌아왔으므로 돌아가는 중인 플래그를 해제
            if (Vector3.Distance(transform.position, originalPosition) < 0.1f)
            {
                isReturning = false;
            }
        }
    }

    // 범위를 벗어났을 때 본래 자리로 돌아가도록 호출할 메서드
    public void ReturnToOriginalPosition()
    {
        isReturning = true;
    }
}

