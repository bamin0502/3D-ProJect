using UnityEngine;
using Data;
using Newtonsoft.Json;

public class MultiBossMelee : MonoBehaviour
{
    public int meleeDamage;
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            if (!MultiScene.Instance.isMasterClient) return;
        if (other.CompareTag("Player"))
        {
             Debug.LogWarning("hit a "+ other.name);
            Debug.Log("근접 데미지 입힘");
            Debug.LogWarning(meleeDamage);
            other.TryGetComponent(out MultiPlayerHealth playerHealth);
            if (playerHealth != null)
            {
                if(playerHealth.CurrentHealth <= 0) return;
                playerHealth.TakeDamage(meleeDamage);
                MultiScene.Instance.BroadCastingTakeDamage(other != null ? other.name : "", meleeDamage);
            }
        }
    }
    private void Start()
    {
        string json = "{\"damage\": 3000}";
        EnemyStat enemyStat1 = JsonConvert.DeserializeObject<EnemyStat>(json);
        meleeDamage = (int)enemyStat1.damage;
    }
}