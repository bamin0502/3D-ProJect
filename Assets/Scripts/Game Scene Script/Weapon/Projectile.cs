using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 25f;
    public int damage = 100;
    public Rigidbody rb;
    private Transform _target;

    private void Start()
    {
        SoundManager.instance.PlaySE("Bow_Attack");
        StartCoroutine(DeleteCoroutine());
    }

    public void Shot(Transform target)
    {
        _target = target;
    }

    void FixedUpdate()
    {
        if (_target != null)
        {
            rb.velocity = transform.forward * speed;
            Vector3 targetPos = _target.transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(targetPos);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator DeleteCoroutine()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_target == null) return;
        
        if (other.CompareTag("Enemy"))
        {
            if (_target != other.transform)
            {
                Destroy(gameObject);
                return;
            }
            
            rb.constraints = RigidbodyConstraints.FreezeAll;
            speed = 0;
            
            _target.TryGetComponent(out EnemyHealth enemy);
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}
