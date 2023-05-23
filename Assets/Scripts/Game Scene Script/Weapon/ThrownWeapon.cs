using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownWeapon : MonoBehaviour
{
    //임성훈
    public float explosionDelay = 2f;
    public float explosionRadius;
    public LayerMask enemyLayer;
    private Collider[] enemies;

    void Start()
    {
        Invoke("Explode", explosionDelay);
    }

    void Explode()
    {
        var size = Physics.OverlapSphereNonAlloc(transform.position, explosionRadius, enemies, enemyLayer);

        foreach (Collider enemy in enemies)
        {
            //나중에 적에게 데미지를 주는 코드를 넣어야함
        }

        Destroy(gameObject);
    }
}
