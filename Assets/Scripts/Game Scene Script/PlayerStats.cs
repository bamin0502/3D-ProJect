using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[System.Serializable]
public class PlayerStats : MonoBehaviour
{
    public string playerName; //플레이어 이름
    public int playerLevel; //플레이어 레벨
    public float health; //체력
    public float attack; //공격력
    public float speed; //이동속도
    public float PlayerExp; //경험치
    void Start()
    {
        
    }
}
