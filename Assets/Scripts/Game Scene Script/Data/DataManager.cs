using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.Events;

class PlayerStat
{
    public string name = "";
    public float Level = 0;
    public float Exp = 0;
    public float PlayerHealth = 0;
    
    public float Health = 0;
}
class EnemyStat
{
    public float EnemyHealth = 0;
    public float damage = 0;
    public float Health = 0;
}
class Itemdata
{
    public string itemName = ""; // 아이템의 이름
    public float Health = 0; //아이템 회복량
    public float damage = 0; //아이템 데미지
    public float dot = 0; //아이템 도트데미지
    public float sight = 0; //아이템 시야설정
}
class WeaponData 
{
    public int damage = 0;
}

public class DataManager : MonoBehaviour
{
    public bool isDead { get; set; }
    [System.NonSerialized]
    public UnityEvent deadEvent = new UnityEvent();
    public static DataManager Inst;
    // Start is called before the first frame update
    void Start()
    {
        var Playerstat = new PlayerStat //플레이어 설정
        {
            name = "",
            Level=1,
            Exp=0,            
            Health = 200,
            PlayerHealth=200
        };
        var Enemystat1 = new EnemyStat //몬스터1 설정
        {
            EnemyHealth=50,
            damage = 20,
            Health = 50
        };
        var Enemystat2 = new EnemyStat //몬스터2 설정
        {
            EnemyHealth = 70,
            damage = 10,
            Health = 70
        };
        var Enemystat3 = new EnemyStat //몬스터3 설정
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
        var weapon = new WeaponData { damage =30};



        //potion의 아이템 값을 읽어옴
        File.WriteAllText(Application.dataPath + "/Itemdata.json", JsonUtility.ToJson(potion));
        Debug.Log(Application.dataPath + "/Itemdata.json");
        //플레이어의 값을 읽어옴
        File.WriteAllText(Application.dataPath + "/PlayerStat.json", JsonUtility.ToJson(Playerstat));
        Debug.Log(Application.dataPath + "/PlayerStat.json");
        //몬스터1의 값을 읽어옴
        File.WriteAllText(Application.dataPath + "/EnemyStat.json", JsonUtility.ToJson(Enemystat1));
        Debug.Log(Application.dataPath + "/EnemyStat.json");
        //몬스터2의 값을 읽어옴
        File.WriteAllText(Application.dataPath + "/EnemyStat.json", JsonUtility.ToJson(Enemystat2));
        Debug.Log(Application.dataPath + "/EnemyStat.json");
        //몬스터3의 값을 읽어옴
        File.WriteAllText(Application.dataPath + "/EnemyStat.json", JsonUtility.ToJson(Enemystat2));
        Debug.Log(Application.dataPath + "/EnemyStat.json");
        //무기의 값을 읽어옴
        File.WriteAllText(Application.dataPath + "/WeaponData.json", JsonUtility.ToJson(weapon));
        Debug.Log(Application.dataPath + "/WeaponData.json");


    }
    //랜덤 공격력 부여
    public int GetRandomDamage()
    {
        string LoadWeaponstat = File.ReadAllText(Application.dataPath + "/WeaponData.json");
        Debug.Log("ReadAllText :" + LoadWeaponstat);
        WeaponData data = JsonUtility.FromJson<WeaponData>(LoadWeaponstat);
        string log = string.Format("data {0}", data.damage);
        Debug.Log(log);
        int randAttack = Random.Range(data.damage, data.damage + 10);
        return randAttack;
    }
    //몬스터가 데미지를 입는 기능
    public void SetEnemyAttack()
    {
        string LoadEnemyStat = File.ReadAllText(Application.dataPath + "/EnemyStat.json");
        Debug.Log("ReadAllText :" + LoadEnemyStat);
        EnemyStat endata = JsonUtility.FromJson<EnemyStat>(LoadEnemyStat);        
        string log = string.Format("data {0},{1}", endata.EnemyHealth,endata.Health);
        Debug.Log(log);

        endata.EnemyHealth = endata.Health - GetRandomDamage();
        UpdateAfterReceiveAttack();
    }
    //플레이어가 데미지를 입는 기능
    public void SetPlayerAttack()
    {
        string LoadEnemyStat = File.ReadAllText(Application.dataPath + "/EnemyStat.json");
        Debug.Log("ReadAllText :" + LoadEnemyStat);
        EnemyStat endata = JsonUtility.FromJson<EnemyStat>(LoadEnemyStat);
        string LoadPlayerstat = File.ReadAllText(Application.dataPath + "/PlayerStat.json");
        Debug.Log("ReadAllText :" + LoadPlayerstat);
        PlayerStat data = JsonUtility.FromJson<PlayerStat>(LoadPlayerstat);
        string log = string.Format("data {0},{1},{2}", data.PlayerHealth, data.Health, endata.damage);

        data.PlayerHealth = data.Health - endata.damage;
        UpdateAfterReceiveAttack();
    }
    protected virtual void UpdateAfterReceiveAttack()
    {
        string LoadPlayerstat = File.ReadAllText(Application.dataPath + "/PlayerStat.json");
        Debug.Log("ReadAllText :" + LoadPlayerstat);
        PlayerStat data = JsonUtility.FromJson<PlayerStat>(LoadPlayerstat);
        string Playerlog = string.Format("data {0}", data.PlayerHealth);
        string LoadEnemyStat = File.ReadAllText(Application.dataPath + "/EnemyStat.json");
        Debug.Log("ReadAllText :" + LoadEnemyStat);
        EnemyStat endata = JsonUtility.FromJson<EnemyStat>(LoadEnemyStat);
        string Enemylog = string.Format("data {0}", endata.EnemyHealth);
        //플레이어 체력이 0이하거나 같으면 사망이벤트 실행
        if (data.PlayerHealth <= 0)
        {
            data.PlayerHealth = 0;
            isDead = true;

            deadEvent.Invoke();
        }
        //몬스터 체력이 0이하거나 같으면 사망이벤트 실행
        if (endata.EnemyHealth <= 0)
        {
            endata.EnemyHealth = 0;
            isDead = true;

            deadEvent.Invoke();
        }
    }
}
