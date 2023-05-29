using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using UnityEngine.UI;

public enum WeaponType
{
    Gun,
    Bow,
    OneHanded,
    TwoHanded,
}

public abstract class Weapon : MonoBehaviour
{
    //임성훈
    public float range;
    public string weaponName; //무기명
    public int damage; //데미지
    public bool isEquipped = false; //장착 여부
    public float attackInterval;
    public WeaponType weaponType;
    public Vector3 PickPosition;
    public Vector3 PickRotation;
    private DataManager dataManager;
    public Canvas iconCanvas;

    private void Awake()
    {
        dataManager=FindObjectOfType<DataManager>();
        iconCanvas = GetComponentInChildren<Canvas>();
    }
    private void Start()
    {
        string json = "{\"damage\": 30}";
        WeaponData weapon = JsonConvert.DeserializeObject<WeaponData>(json);
        damage = weapon.damage;
    }
    public abstract void Attack(Transform target);
    public void EnableCanvas()
    {
        if (iconCanvas != null)
        {
            iconCanvas.enabled = true;
        }
    }

    public void DisableCanvas()
    {
        if (iconCanvas != null)
        {
            iconCanvas.enabled = false;
        }
    }
}
