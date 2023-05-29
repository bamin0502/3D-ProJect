using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bullet : MonoBehaviour
{
    public Transform target;
    public int damage;
    public float speed = 10f;  // 미사일 속도
      
    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;  // Player 태그가 달린 오브젝트를 타겟으로 설정
    }

    private void Update()
    {
        Destroy(gameObject, 5f);
        if (target == null)  // 타겟이 없으면 미사일 삭제
        {
            Destroy(gameObject);
            return;
        }

        // 타겟 방향으로 이동
        Vector3 direction = target.position - transform.position;
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        // 미사일이 타겟에 도달하면 충돌 처리
        float distanceToTarget = direction.magnitude;
        if (distanceToTarget < 0.5f)  // 예시로 거리 0.5f 이내로 충돌을 감지하도록 설정
        {
            HitTarget();
        }
    }

    private void HitTarget()
    {
        
        DamagePlayer();

        Destroy(gameObject);
    }

    private void DamagePlayer()
    {
        // 타겟에게 데미지를 주는 로직을 구현하세요
        // 예시로는 Player 태그가 달린 오브젝트의 Health 스크립트에 접근하여 데미지를 적용합니다
        GameObject player = GameObject.FindGameObjectWithTag("Player");       

        
    }
}
