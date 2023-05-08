using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    //임성훈
    public float range;

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
