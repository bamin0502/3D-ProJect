using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : Weapon
{
    //임성훈 
    public GameObject projectilePrefab; //발사체 프리팹

    public override void Attack(Transform target)
    {
        float distance = Vector3.Distance(transform.position, target.position);
        
        if (distance <= range)
        {
            // 데미지 처리 로직
            Debug.Log("Damage: " + target.name + damage);
        }
    }
}
