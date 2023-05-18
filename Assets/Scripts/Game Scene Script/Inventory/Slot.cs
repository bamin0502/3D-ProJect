using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;


public class Slot : MonoBehaviour,IPointerClickHandler
{
    public Item item; //획득한 아이템을 기록
    public int ItemCount; //획득한 아이템의 개수를 기록할거임
    public Image itemImage; //아이템의 이미지를 가져올거임

    [SerializeField] private TMP_Text text_Count;//아이템의 개수를 표시할 텍스트 지정
    [SerializeField] private GameObject go_CountImage;
    private DataManager theItemEffectDatabase;
    void Start()
    {
        theItemEffectDatabase=FindObjectOfType<DataManager>();
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
        if(item.itemType ==Item.ItemType.Used && item.itemType == Item.ItemType.Throw)
        {
            go_CountImage.SetActive(true);
            text_Count.text=ItemCount.ToString();
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
    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button==PointerEventData.InputButton.Right)
        {
            if(item != null)
            {
                theItemEffectDatabase.UseItem(item);
                if (item.itemType == Item.ItemType.Used)
                {
                    
                    SetSlotCount(-1);
                }
            }
        }
    }
}
