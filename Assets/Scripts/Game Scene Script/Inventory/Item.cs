using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Item", menuName = "New Item/item")]
public class Item : ScriptableObject
{
    [TextArea]
    public string itemDesc;
    public enum ItemType 
    { 
        Used,
        Throw,
        Buff
    }
    public string itemName;
    public ItemType itemType;
    public Sprite itemImage;
    public GameObject itemPrefab;
    public int cooldownTime=0;

    public string weaponType;
    
}
