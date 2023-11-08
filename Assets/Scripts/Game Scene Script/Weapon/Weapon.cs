using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    protected int skillDamage;
    public bool isEquipped = false; //장착 여부
    public float attackInterval;
    public WeaponType weaponType;
    public Vector3 PickPosition;
    public Vector3 PickRotation;
    private DataManager dataManager;
    public Canvas iconCanvas;
    private WeaponData weaponData;
    private string currentSceneName;
    private void Awake()
    {
        dataManager = FindObjectOfType<DataManager>();
        iconCanvas = GetComponentInChildren<Canvas>();
    }
    private void Start()
    {
        currentSceneName= SceneManager.GetActiveScene().name;
        LoadWeaponData(currentSceneName);
    }

    public int GetDamage()
    {
        return damage;
    }

    public int GetSkillDamage()
    {
        return skillDamage;
    }
    private void LoadWeaponData(string sceneName)
    {
        if (sceneName.Equals("Game Scene"))
        {
            string json = "";

            if (weaponType == WeaponType.Bow)
            {
                json = "{\"damage\": 800, \"skillDamage\": 1800}";
            }
            else if (weaponType == WeaponType.OneHanded)
            {
                json = "{\"damage\": 1350, \"skillDamage\": 2200}";
            }
            else if (weaponType == WeaponType.TwoHanded)
            {
                json = "{\"damage\": 1500, \"skillDamage\": 2500}";
            }

            WeaponData weaponData = JsonConvert.DeserializeObject<WeaponData>(json);
            damage = weaponData.damage;
            skillDamage = weaponData.skillDamage;
        }
    }
    public abstract void Attack(Transform target, int isSkill);
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
