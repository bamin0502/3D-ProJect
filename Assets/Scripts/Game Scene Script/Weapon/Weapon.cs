using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    //임성훈 
    public string weaponName; //무기명
    public int damage; //데미지
    public bool isEquipped = false; //장착 여부
    public float attackInterval;


    public abstract void Attack(Transform target);
}
