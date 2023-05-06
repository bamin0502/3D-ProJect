using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.IO;

[Serializable]
public class Itemdata
{
    public string itemName=""; // 아이템의 이름
    public Sprite itemImage; // 아이템의 이미지(인벤 토리 안에서 띄울)
    public float health; //아이템 회복량
    public float damage; //아이템 데미지
    public float dot; //아이템 도트데미지
    public float sight; //아이템 시야설정
}


[CreateAssetMenu(fileName = "CreateItem", menuName = "Item")]
public class Item : ScriptableObject
{

    public enum ItemType // 아이템 유형
    {
        Countable,  //쌓을 수 있는 아이템
        NoneCountable, //쌓을 수 없는 아이템
    }

    public Action OnPickUp; //줍기 기능
    public string itemName; // 아이템의 이름
    public ItemType itemType; // 아이템 유형
    public Sprite itemImage; // 아이템의 이미지(인벤 토리 안에서 띄울)
    public GameObject itemPrefab; // 아이템의 프리팹 (아이템 생성시 프리팹으로 찍어냄)
}



