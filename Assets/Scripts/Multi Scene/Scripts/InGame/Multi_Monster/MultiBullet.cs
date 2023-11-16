using System.Collections;
using UnityEngine;
using Data;
using Newtonsoft.Json;

public class MultiBullet : MonoBehaviour
{
    public Transform target;
    public int damage;
    public float speed = 3f; // 미사일 속도
    public float collisionDistance=0.5f;
    private void Start()
    {
        StartCoroutine(DeleteMySelf());

        string json = "{\"damage\": 500}";
        try
        {
            EnemyStat enemyStat1 = JsonConvert.DeserializeObject<EnemyStat>(json);
            damage = (int)enemyStat1.damage;
        }
        catch(JsonException e)
        {
            Debug.LogError("Json 역직렬화에 오류가 발생했습니다!"+e.Message);
        }

    }
    public float selfDestructTime = 3f;
    IEnumerator DeleteMySelf()
    {
        yield return new WaitForSeconds(selfDestructTime);
        Destroy(gameObject);
    }

    private void Update()
    {
        if (target == null) // 타겟이 없으면 미사일 삭제
        {
            Destroy(gameObject);
            return;
        }

        // 타겟 방향으로 이동
        Vector3 direction = target.position - transform.position;
        transform.Translate(direction.normalized * (speed * Time.deltaTime), Space.World);
      
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        // 미사일이 타겟에 도달하면 충돌 처리
        float distanceToTarget = direction.magnitude;
        
        if (distanceToTarget < collisionDistance) 
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
        bool isPlayer = target.TryGetComponent(out MultiPlayerHealth playerHealth);
        if (isPlayer && playerHealth != null && playerHealth.CurrentHealth > 0)
        {
            playerHealth.TakeDamage(damage);
            MultiScene.Instance.BroadCastingTakeDamage(target.name, damage);
        }
    }
}