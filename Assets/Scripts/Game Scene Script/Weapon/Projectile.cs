using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float speed = 25f;
    private int damage = 10;

    private void Start(){
        StartCoroutine(DeleteCoroutine());
    }

    private IEnumerator DeleteCoroutine(){
        yield return new WaitForSeconds(5f);

        if (gameObject != null){
            Destroy(gameObject);
        }
    }


    public IEnumerator ShotCoroutine(Transform target){
        if(target == null){
            yield break;
        }

        yield return new WaitForSeconds(0.3f);

        while(target != null && Vector3.Distance(transform.position, target.position) > 0.1f){
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            yield return null;
        }

        if(target != null){
            bool isEnemy = target.TryGetComponent(out EnemyHealth enemy);
            if (isEnemy)
            {
                enemy.TakeDamage(damage);
            }
            
            if (gameObject != null){
                Destroy(gameObject);
            }
        }
    }

}
