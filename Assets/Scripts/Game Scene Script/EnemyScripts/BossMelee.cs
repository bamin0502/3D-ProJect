using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using Newtonsoft.Json;

public class BossMelee : MonoBehaviour
{
    private PlayerHealth playerHealth;
    public int meleeDamage;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // 플레이어 태그를 가진 오브젝트와 충돌한 경우

            playerHealth.TakeDamage(meleeDamage);
        }
    }
    private void Start()
    {
        string json = "{\"damage\": 50}";
        EnemyStat enemyStat1 = JsonConvert.DeserializeObject<EnemyStat>(json);
        meleeDamage = (int)enemyStat1.damage;
    }
}
