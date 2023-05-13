using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.Events;

public class PlayerStat
{
    public string name = "";
    public float Level = 0;
    public float Exp = 0;
    public float PlayerHealth = 0;
    public float Health = 0;
}

public class EnemyStat
{
    public float EnemyHealth = 0;
    public float damage = 0;
    public float Health = 0;
}

public class Itemdata
{
    public string itemName = ""; // 아이템의 이름
    public float Health = 0; //아이템 회복량
    public float damage = 0; //아이템 데미지
    public float dot = 0; //아이템 도트데미지
    public float sight = 0; //아이템 시야설정
}

public class WeaponData
{
    public int damage = 0;
}

public class DataManager : MonoBehaviour
{
    public bool isDead { get; set; }
    [System.NonSerialized]
    public UnityEvent deadEvent = new UnityEvent();
    public static DataManager Inst;

    private void Start()
    {
        InitializeData();
        SaveData();
    }
    //여기다가 var타입으로 만들고 SaveToJson방식으로 저장시킬거임
    private void InitializeData()
    {
        var playerStat = new PlayerStat
        {
            name = "",
            Level = 1,
            Exp = 0,
            Health = 200,
            PlayerHealth = 200
        };

        var enemyStat1 = new EnemyStat
        {
            EnemyHealth = 50,
            damage = 20,
            Health = 50
        };

        var enemyStat2 = new EnemyStat
        {
            EnemyHealth = 70,
            damage = 10,
            Health = 70
        };

        var enemyStat3 = new EnemyStat
        {
            EnemyHealth = 30,
            damage = 30,
            Health = 30
        };

        var potion = new Itemdata
        {
            itemName = "Potion",
            Health = 30
        };

        var weapon = new WeaponData { damage = 30 };

        SaveToJson(potion, "Itemdata.json");
        SaveToJson(playerStat, "PlayerStat.json");
        SaveToJson(enemyStat1, "EnemyStat1.json");
        SaveToJson(enemyStat2, "EnemyStat2.json");
        SaveToJson(enemyStat3, "EnemyStat3.json");
        SaveToJson(weapon, "WeaponData.json");
    }
    //자동으로 저장시켜줄거임
    private void SaveToJson<T>(T data, string fileName)
    {
        string jsonData = JsonConvert.SerializeObject(data);
        string filePath = Application.dataPath + "/" + fileName;
        File.WriteAllText(filePath, jsonData);
        Debug.Log("Saved JSON data to: " + filePath);
    }
    //자동으로 로딩시킬거임
    private T LoadFromJson<T>(string fileName)
    {
        string filePath = Application.dataPath + "/" + fileName;
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            T data = JsonConvert.DeserializeObject<T>(jsonData);
            return data;
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
            return default(T);
        }
    }
    //랜덤 데미지 함수(고정형 데미지가 아닌 랜덤으로 데미지를 줄거임)
    public int GetRandomDamage()
    {
        WeaponData weaponData = LoadFromJson<WeaponData>("WeaponData.json");
        int randAttack = Random.Range(weaponData.damage, weaponData.damage + 10);
        return randAttack;
    }
    //몬스터에게 데미지를 줄 기능
    public void SetEnemyAttack()
    {
        EnemyStat enemyStat = LoadFromJson<EnemyStat>("EnemyStat1.json");
        enemyStat.EnemyHealth -= GetRandomDamage();
        UpdateAfterReceiveEnemyAttack(enemyStat);
    }
    //플레이어가 데미지를 받을 기능
    public void SetPlayerAttack()
    {
        EnemyStat enemyStat = LoadFromJson<EnemyStat>("EnemyStat1.json");
        PlayerStat playerStat = LoadFromJson<PlayerStat>("PlayerStat.json");
        playerStat.PlayerHealth -= enemyStat.damage;
        UpdateAfterReceivePlayerAttack(playerStat);
    }

    protected virtual void UpdateAfterReceiveEnemyAttack(EnemyStat enemyStat)
    {
        if (enemyStat.EnemyHealth <= 0)
        {
            enemyStat.EnemyHealth = 0;
            isDead = true;
            deadEvent.Invoke();
        }
        
    }
    protected virtual void UpdateAfterReceivePlayerAttack(PlayerStat playerStat)
    {
        if (playerStat.PlayerHealth <= 0)
        {
            playerStat.PlayerHealth = 0;
            isDead = true;
            deadEvent.Invoke();
        }
    }
    private void SaveData()
    {
        // PlayerStat 저장
        PlayerStat playerStat = LoadFromJson<PlayerStat>("PlayerStat.json");
        SaveToJson(playerStat, "PlayerStat.json");

        // EnemyStat 저장
        EnemyStat enemyStat1 = LoadFromJson<EnemyStat>("EnemyStat1.json");
        SaveToJson(enemyStat1, "EnemyStat1.json");

        // WeaponData 저장
        WeaponData weaponData = LoadFromJson<WeaponData>("WeaponData.json");
        SaveToJson(weaponData, "WeaponData.json");

        //뒤에 만들것도 위랑 같은 방식으로 만들면 됩니다.

        // 추가적인 데이터 저장 로직을 여기에 구현하면 됩니다.

        Debug.Log("Data saved successfully.");
    }
    public void LevelUP()
    {
        // TODO: Implement level up logic
    }
}