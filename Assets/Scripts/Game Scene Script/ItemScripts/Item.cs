using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.IO;

[CreateAssetMenu(fileName = "CreateItem", menuName = "Item")]
public class Item : ScriptableObject
{
    public enum ItemType // 아이템 유형
    {
        Countable,  //쌓을 수 있는 아이템
        NoneCountable, //쌓을 수 없는 아이템
    }
    public void UsePotion()
    {
        string LoadItemdata = File.ReadAllText(Application.dataPath + "/Itemdata.json");
        Debug.Log("ReadAllText :" + LoadItemdata);
        string LoadPlayerstat = File.ReadAllText(Application.dataPath + "/PlayerStat.json");
        Itemdata data = JsonUtility.FromJson<Itemdata>(LoadItemdata);
        PlayerStat playdata = JsonUtility.FromJson<PlayerStat>(LoadPlayerstat);
        //물약의 아이템이름, 물약의 회복량을 가져옴
        string log = string.Format("data {0},{1}", data.itemName, data.Health);
        //플레이어의 체력을 가져옴
        string enlog = string.Format("data {0}", playdata.Health);
        playdata.Health += data.Health;
        //기존 플레이어 체력보다 더 많이 회복을 할수 없도록 제한
        if (playdata.Health <= (playdata.Health += data.Health))
        {
            Debug.Log("더 이상 회복할수 없습니다! 이미 체력이 최대입니다.");
        }
    }
    public Action OnPickUp; //줍기 기능
    public string itemName; // 아이템의 이름
    public ItemType itemType; // 아이템 유형
    public Sprite itemImage; // 아이템의 이미지(인벤 토리 안에서 띄울)
    public GameObject itemPrefab; // 아이템의 프리팹 (아이템 생성시 프리팹으로 찍어냄)
}



