using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;



class PlayerStat
{
    public float PlayerHealth = 0;
    public float damage = 0;
    public float Health = 0;
}
class EnemyStat
{
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

public class DataManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var Playerstat = new PlayerStat //플레이어 설정
        {
            damage = 20,
            Health = 200,
            PlayerHealth=200
        };
        var Enemystat1 = new EnemyStat //몬스터1 설정
        {
            damage = 20,
            Health = 50
        };
        var Enemystat2 = new EnemyStat //몬스터2 설정
        {
            damage = 10,
            Health = 70
        };
        var Enemystat3 = new EnemyStat //몬스터3 설정
        {
            damage = 30,
            Health = 30

        };
        var potion = new Itemdata
        {
            itemName = "Potion",
            Health = 30
        };

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
    }

}
