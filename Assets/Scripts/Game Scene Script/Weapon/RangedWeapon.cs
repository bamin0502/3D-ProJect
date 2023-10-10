using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : Weapon
{
    //임성훈
    public Transform arrowPos;
    public GameObject projectilePrefab; //발사체 프리팹

    public override void Attack(Transform target, int isSkill)
    {
    }
}
