using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownWeapon : MonoBehaviour
{
    //임성훈
    public float explosionRadius;  //폭발 반경
    public LayerMask enemyLayer;  //적 레이어
    public int damage = 50; //데미지
    public float delay = 3f;
    private Collider[] enemies = new Collider[20]; //최대 20마리까지 데미지 줌

    void Start()
    {
        StartCoroutine(ExplosionAfterDelay());
    }

    private void OnTriggerEnter(Collider ground)
    {
        if (ground.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Explode();
        }
    }
    
    IEnumerator ExplosionAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        Explode();
    }

    void Explode()
    {
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, explosionRadius, enemies, enemyLayer);

        for (int i = 0; i < numColliders; i++)
        {
            Collider target = enemies[i];

            bool isEnemy = target.TryGetComponent(out EnemyHealth enemy);
            if (isEnemy)
            {
                enemy.TakeDamage(damage);
            }
        }
        
        Destroy(gameObject);
    }
}
