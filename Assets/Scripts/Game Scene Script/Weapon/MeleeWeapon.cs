using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using Newtonsoft.Json;

public class MeleeWeapon : Weapon
{
    public override int GetDamage()
    {
        return damage; // 현재 공격력 값을 반환
    }

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
