using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerClickHandler
{
    //임성훈

    public Item item;
    public int itemCount; //아이템 개수
    public Image itemImage; //아이템 이미지
    public TextMeshProUGUI itemCountText; //아이템 개수 텍스트

    
    private void SetColor(Image _image, float _alpha) //이미지 투명도 조절
    {
        Color color = _image.color;
        color.a = _alpha;
        _image.color = color;
    }

    public void AddItem(Item _item, int _count = 1) //아이템 count개수만큼 추가(기본 1)
    {
        item = _item;
        itemCount = _count;
        itemImage.sprite = item.itemImage;

        if (item.itemType != Item.ItemType.NoneCountable)
        {
            itemCountText.gameObject.SetActive(true);
            itemCountText.text = itemCount.ToString();
        }
        else
        {
            itemCountText.gameObject.SetActive(false);
        }

        SetColor(itemImage, 1);
    }
    
    public void SetSlotCount(int _count) //아이템 개수 조절
    {
        itemCount += _count;
        itemCountText.text = itemCount.ToString();
        
        if (itemCount <= 0)
            ClearSlot();
    }
    
    private void ClearSlot() //슬롯 리셋
    {
        item = null;
        itemCount = 0;
        itemImage.sprite = null;
        itemCountText.text = "";
        SetColor(itemImage, 0);
        itemCountText.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData) //슬롯 클릭시
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (item != null)
            {
                //아이템 사용함수 작성
            }
            else
            {
                //아이템 슬롯 비어있을 경우 할 행동
            }
        }
    }
}
