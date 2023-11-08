using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Data;

public class ThrownWeapon : MonoBehaviour
{
    //임성훈
    public float explosionRadius;  //폭발 반경
    public LayerMask enemyLayer;  //적 레이어
    public int damage; //데미지
    public float fireDuration = 3f;  //화염 지속시간
    private readonly Collider[] enemies = new Collider[20]; //최대 20마리까지 데미지 줌
    [SerializeField] private GameObject fireEffect;  //화염 이펙트
    [SerializeField] private GameObject explosionEffect;  //폭발 이펙트
    public bool canExplode; //폭발할 수 있는 상태인가
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;
        canExplode = false;
    }

    void Start()
    {
        StartCoroutine(CanExplode());
        string json = "{\"damage\": 200}";
        Itemdata FireGrenade = JsonConvert.DeserializeObject<Itemdata>(json);
        damage = (int)FireGrenade.damage;
    }

    private IEnumerator CanExplode()
    {
        yield return new WaitForSeconds(1f);
        canExplode = true;
    }

    private void OnTriggerEnter(Collider ground)
    {
        if (ground.gameObject.layer == LayerMask.NameToLayer("Object") && canExplode)
        {
            Explode();
        }
    }

    void Explode()
    {
        var position = transform.position;
        Instantiate(explosionEffect, position, Quaternion.identity);
        GameObject fire = Instantiate(fireEffect, position, Quaternion.identity);
        fire.transform.localScale = new Vector3(explosionRadius, 1, explosionRadius);

        int numColliders = Physics.OverlapSphereNonAlloc(position, explosionRadius, enemies, enemyLayer);

        for (int i = 0; i < numColliders; i++)
        {
            Collider target = enemies[i];
            bool isEnemy = target.TryGetComponent(out EnemyHealth enemy);
            if (isEnemy)
            {
                StartCoroutine(ApplyDoT(enemy, fireDuration, fire));
            }
        }

        SoundManager.instance.PlaySE("Fire_Duration");
        SoundManager.instance.PlaySE("Bottle_Break");

        Destroy(gameObject, fireDuration);
        Destroy(fire, fireDuration);
    }

    IEnumerator ApplyDoT(EnemyHealth enemy, float duration, GameObject fireEffect)
    {
        float applyDamageInterval = 0.3f;
        float time = 0.0f;

        while (time < duration)
        {
            if (Vector3.Distance(enemy.transform.position, fireEffect.transform.position) <= explosionRadius)
            {
                enemy.TakeDamage(damage);
            }

            time += applyDamageInterval;
            yield return new WaitForSeconds(applyDamageInterval);
        }
        SoundManager.instance.StopSE("Fire_Duration");
    }

    private void OnDestroy()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.StopSE("Fire_Duration");
        }
        
    }
}
