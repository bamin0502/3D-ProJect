using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using Newtonsoft.Json;

public class BossMelee : MonoBehaviour
{
    public bool isAttacking = false;
    private PlayerHealth playerHealth;
    private Transform target;
    public int meleeDamage;
    void OnTriggerEnter(Collider other)
    {
        if (isAttacking && other.gameObject.CompareTag("Player"))
        {

           
            bool isPlayer = target.TryGetComponent(out PlayerHealth playerHealth);
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
