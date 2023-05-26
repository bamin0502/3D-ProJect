using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    //임성훈
    public override void Attack(Transform target)
    {
        bool isEnemy = target.TryGetComponent(out EnemyHealth enemy);
        if (isEnemy)
        {
            enemy.TakeDamage(damage);
        }
    }
}
