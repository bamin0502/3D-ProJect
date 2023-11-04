using UnityEngine;
using Data;
using Newtonsoft.Json;

public class BossMelee : MonoBehaviour
{
    public bool isAttacking = false;
    private PlayerHealth playerHealth;
    public int meleeDamage;
    private Transform target;
    void OnTriggerEnter(Collider other)
    {
        if (isAttacking && other.gameObject.CompareTag("Player"))
        {
            // 플레이어 태그를 가진 오브젝트와 충돌한 경우
            
            bool isPlayer = target.TryGetComponent(out MultiPlayerHealth playerHealth);
            if (isPlayer)
            {
                Debug.Log("근접 데메지 입힘");
                playerHealth.TakeDamage(meleeDamage);
            }
        }
    }
    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").gameObject.transform;
        string json = "{\"damage\": 80}";
        EnemyStat enemyStat1 = JsonConvert.DeserializeObject<EnemyStat>(json);
        meleeDamage = (int)enemyStat1.damage;
    }
}
