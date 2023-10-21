using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 25f;
    public int damage = 100;
    private bool isDestroy = false;
    
    private void Start(){
        StartCoroutine(DeleteCoroutine());
        
    }

    private IEnumerator DeleteCoroutine(){
        yield return new WaitForSeconds(5f);
        isDestroy = true;
        Destroy(gameObject);
    }


    public IEnumerator ShotCoroutine(Transform target){
        if(target == null){
            yield break;
        }

        yield return new WaitForSeconds(0.3f);
        SoundManager.instance.PlaySE("Bow_Attack");

        while(target != null && Vector3.Distance(transform.position, target.position) > 0.1f){
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            yield return null;
        }

        if(target != null){
            bool isEnemy = target.TryGetComponent(out EnemyHealth enemy);
            if (isEnemy)
            {
                enemy.TakeDamage(damage, transform.position);
            }

            isDestroy = true;
            Destroy(gameObject);
        }
    }
}
