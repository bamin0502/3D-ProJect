using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using Newtonsoft.Json;

public class MeleeWeapon : Weapon
{
    private void Start()
    {
        string json = "{\"damage\": 30}";
        WeaponData weapon = JsonConvert.DeserializeObject<WeaponData>(json);
        damage = weapon.damage;
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
