using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "New Item/item")]
public class Item : ScriptableObject
{
    //임성훈

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
