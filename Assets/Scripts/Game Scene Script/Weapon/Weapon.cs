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
    protected int damage; //데미지
    public bool isEquipped = false; //장착 여부
    public float attackInterval;
    public WeaponType weaponType;
    public Vector3 PickPosition;
    public Vector3 PickRotation;
    private DataManager dataManager;
    public Canvas iconCanvas;
    private WeaponData weaponData;
    
    private void Awake()
    {
        dataManager=FindObjectOfType<DataManager>();
        iconCanvas = GetComponentInChildren<Canvas>();

    }
    private void Start()
    {
        LoadWeaponData();
    }
    private void LoadWeaponData()
    {
        string json = "";

        if (weaponType == WeaponType.Bow)
        {
            json = "{\"damage\": 20}";
        }
        else if (weaponType == WeaponType.OneHanded)
        {
            json = "{\"damage\": 40}";
        }
        else if (weaponType == WeaponType.TwoHanded)
        {
            json = "{\"damage\": 60}";
        }
        WeaponData weaponData = JsonConvert.DeserializeObject<WeaponData>(json);
        damage = weaponData.damage;


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
