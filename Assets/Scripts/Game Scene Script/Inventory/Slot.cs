using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;
using Data;


public class Slot : MonoBehaviour, /*IPointerEnterHandler, IPointerExitHandler,*/ IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Item item; //획득한 아이템을 기록
    public int ItemCount; //획득한 아이템의 개수를 기록할거임
    public Image itemImage; //아이템의 이미지를 가져올거임

    [SerializeField] private TMP_Text text_Count;//아이템의 개수를 표시할 텍스트 지정
    [SerializeField] private GameObject go_CountImage;
    private DataManager theItemEffectDatabase;
    [SerializeField] private Slot[] slots;
    [SerializeField] private GameObject slot1;
    [SerializeField] private GameObject slot2;
    [SerializeField] private GameObject slot3;
    [SerializeField] private GameObject slot4;
    public int GetSlotCount()
    {
        return ItemCount;
    }
    void Start()
    {
        theItemEffectDatabase = FindObjectOfType<DataManager>();
        slots = new Slot[4];
        if (MultiScene.Instance.inventory.slots.Length >= 4)
        {
            slots[0] = slot1.GetComponent<Slot>();
            slots[1] = slot2.GetComponent<Slot>();
            slots[2] = slot3.GetComponent<Slot>();
            slots[3] = slot4.GetComponent<Slot>(); 
        }
        
    }
    void Update()
    {
        TryInputNumber();
    }
    private void TryInputNumber()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && slots.Length > 0)
        {
            UseItemInSlot(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && slots.Length > 1)
        {
            UseItemInSlot(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && slots.Length > 2)
        {
            UseItemInSlot(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) && slots.Length > 3)
        {
            UseItemInSlot(3);
        }
    }

    internal void UseItemInSlot(int slotIndex)
    {
        if(slots[slotIndex] == null) return;
        if (slots[slotIndex].item != null)
        {
            bool itemUsed = theItemEffectDatabase.UseItem(slots[slotIndex].item);

            if (!itemUsed) return;
            
            slots[slotIndex].SetSlotCount(-1);
        }
        else
        {
            Debug.Log("아이템 사용에 실패했습니다.");
        }
        
    }
    private void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }
    public void AddItem(Item _item, int _count = 1)
    {
        item = _item;
        ItemCount = _count;
        itemImage.sprite = item.itemImage;
        //아이템의 타입이 사용할수 있는 타입이거나 던질수 있는 타입이면
        if (item.itemType == Item.ItemType.Used || item.itemType == Item.ItemType.Throw || item.itemType ==Item.ItemType.Buff)
        {
            go_CountImage.SetActive(true);
            text_Count.text = ItemCount.ToString();
            
        }
        if(item.itemType == Item.ItemType.Throw)
        {
            SoundManager.instance.PlaySE("Item Drop");
        }

        SetColor(1);
    }
    public void SetSlotCount(int _count)
    {
        ItemCount += _count;
        text_Count.text = ItemCount.ToString();
        if (ItemCount <= 0)
            ClearSlot();
    }

    private void ClearSlot()
    {
        item = null;
        ItemCount = 0;
        itemImage.sprite = null;
        SetColor(0);

        text_Count.text = "0";
        go_CountImage.SetActive(false);
    }
    //아이템이 있는 슬롯을 클릭했을때 호출할 이벤트
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (item != null)
            {                
                bool itemUsed=theItemEffectDatabase.UseItem(item);
                if(itemUsed)
                {
                    if (item.itemType == Item.ItemType.Used || item.itemType == Item.ItemType.Buff || item.itemType==Item.ItemType.Throw)
                    {
                        SetSlotCount(-1);
                    }
                }
                else
                {
                    Debug.Log("아이템 사용에 실패했습니다.");
                }
            }
        }        
    }
    //아이템이 있는 슬롯을 처음 드래그할때 호출할 이벤트
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(item != null)
        {
            DragSlot.inst.dragSlot = this;
            DragSlot.inst.DragSetImage(itemImage);
            DragSlot.inst.transform.position = eventData.position;
        }
    }
    //아이템이 있는 슬롯을 드래그 중일때 호출할 이벤트
    public void OnDrag(PointerEventData eventData)
    {
        if(item != null)
        {
            DragSlot.inst.transform.position = eventData.position;
        }
    }
    //아이템이 있던 슬롯을 드래그가 끝났을때 호출할 이벤트 
    public void OnEndDrag(PointerEventData eventData)
    {
        DragSlot.inst.SetColor(0);
        DragSlot.inst.dragSlot = null;
        SoundManager.instance.StopSE("Item Drop");
    }
    //해당 빈 슬롯에 무언가가 마우스 드롭 되었을때 발생할 이벤트
    public void OnDrop(PointerEventData eventData)
    {
        if (DragSlot.inst.dragSlot != null)
            ChangeSlot();
    }
    //슬롯의 내용을 서로 바꾼다.
    private void ChangeSlot()
    {
        Item _tempItem = item;
        int _tempItemCount = ItemCount;
       
        AddItem(DragSlot.inst.dragSlot.item, DragSlot.inst.dragSlot.ItemCount);

        if (_tempItem != null)
            DragSlot.inst.dragSlot.AddItem(_tempItem, _tempItemCount);
        else
            DragSlot.inst.dragSlot.ClearSlot();
    }
    //// 마우스 커서가 슬롯에 들어갈 때 발동
    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    if (item != null)
    //        theItemEffectDatabase.ShowToolTip(item, transform.position);
    //}

    //// 마우스 커서가 슬롯에서 나올 때 발동
    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    theItemEffectDatabase.HideToolTip();
    //}
    private void ThrowItem(Item _item)
    {
        // 마우스 포인트를 기준으로 아이템을 던지는 로직 작성
        // 예시: 아이템의 던지는 동작을 애니메이션으로 표현하거나, 특정 위치에 아이템을 생성하는 등의 처리
        Vector3 mousePosition = Input.mousePosition;
        // 마우스 포인트를 월드 좌표로 변환
        if (Camera.main != null)
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        }
        // 아이템을 해당 위치에 던지는 동작 수행
        //_item.ThrowItem(worldPosition);
    }
}