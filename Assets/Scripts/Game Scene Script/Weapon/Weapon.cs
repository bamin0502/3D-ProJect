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
        dataManager=FindObjectOfType<DataManager>();
        iconCanvas = GetComponentInChildren<Canvas>();

    }
    private void Start()
    {
        currentSceneName= SceneManager.GetActiveScene().name;
        LoadWeaponData(currentSceneName);
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
                json = "{\"damage\": 200, \"skillDamage\": 400}";
            }
            else if (weaponType == WeaponType.OneHanded)
            {
                json = "{\"damage\": 350, \"skillDamage\": 600}";
            }
            else if (weaponType == WeaponType.TwoHanded)
            {
                json = "{\"damage\": 600, \"skillDamage\": 800}";
            }

            WeaponData weaponData = JsonConvert.DeserializeObject<WeaponData>(json);
            damage = weaponData.damage;
            skillDamage = weaponData.skillDamage;
        }
        // else if (sceneName.Equals("Single Scene"))
        // {
        //     string json = "";
        //
        //     if (weaponType == WeaponType.Bow)
        //     {
        //         json = "{\"damage\": 20, \"skillDamage\": 60}";
        //     }
        //     else if (weaponType == WeaponType.OneHanded)
        //     {
        //         json = "{\"damage\": 35, \"skillDamage\": 80}";
        //     }
        //     else if (weaponType == WeaponType.TwoHanded)
        //     {
        //         json = "{\"damage\": 60, \"skillDamage\": 120}";
        //     }
        //
        //     WeaponData weaponData = JsonConvert.DeserializeObject<WeaponData>(json);
        //     damage = weaponData.damage;
        //     skillDamage = weaponData.skillDamage;
        // }
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
