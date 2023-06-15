using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Inventory : MonoBehaviour
{
    //인벤토리 활성화 여부를 설정한다, 여기서는 닫지않을거여서 항상 True로 지정
    public static bool inventoryActivated = true;
    [SerializeField]
    private GameObject go_InventoryBase;
    [SerializeField]
    private GameObject go_SlotsParent;
    //슬롯들의 배열 지정
    private Slot[] slots;
    // Start is called before the first frame update
    void Start()
    {

        //부모의 자식값을 가져옴
        slots = go_SlotsParent.GetComponentsInChildren<Slot>();
    }
    public void AcquireItem(Item _item, int _count = 1)
    {
        // 이미 아이템이 있는데 그 아이템 타입이 Used나 Throw형일경우
        if (Item.ItemType.Used == _item.itemType || Item.ItemType.Throw == _item.itemType || Item.ItemType.buff ==_item.itemType)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item != null)
                {
                    if (slots[i].item.itemName == _item.itemName)
                    {
                        slots[i].SetSlotCount(_count);
                        return;
                    }
                }
            }
        }
        //아이템이 없다면
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].AddItem(_item, _count);
                return;
            }
        }
        
    }
}