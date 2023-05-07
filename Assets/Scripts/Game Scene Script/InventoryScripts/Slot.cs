using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerClickHandler
{
    public Item item;
    public int itemCount; //아이템 개수
    public Image itemImage; //아이템 이미지
    public TextMeshProUGUI itemCountText; //아이템 개수 텍스트
    public TMP_Text NoticeText;
    [SerializeField]
    //슬롯에 번호를 매겨서 각각 번호로 눌러서 사용할수 있게 만들거임 그 전에 먼저 키보드 번호를 정해준다.
    private static KeyCode ItemSlotnum1, ItemSlotnum2, ItemSlotnum3, ItemSlotnum4, ItemSlotnum5, ItemSlotnum6;
    public Image GButtonImage;
    public Image CoolTimeImage;
    private float CoolTime;
    private float currentCoolTime;
    private bool isCoolTime;

    //마우스 드래그가 끝날때 발생할 이벤트를 만들거임
    private Rect SlotGroup; //Slot의 부모인 HorizontalLayoutGroup 정보를 받아 옴.


    void Start()
    {
        ItemSlotnum1 = KeyCode.Alpha1;
        ItemSlotnum2 = KeyCode.Alpha2;
        ItemSlotnum3 = KeyCode.Alpha3;
        ItemSlotnum4 = KeyCode.Alpha4;
        ItemSlotnum5 = KeyCode.Alpha5;
        ItemSlotnum6 = KeyCode.Alpha6;
        //SlotGroup = transform.parent.parent.GetComponent<RectTransform>().rect;
        //CoolTimeImage.transform.gameObject.SetActive(false);
        //GButtonImage.transform.gameObject.SetActive(false);
    }
    //드래그 밖으로 아이템이 나갈시에는 아이템을 파괴시킬거임
    public void OnEndDrag(PointerEventData eventData)
    {
        
    }
    private void SetColor(Image _image, float _alpha) //이미지 투명도 조절
    {
        Color color = _image.color;
        color.a = _alpha;
        _image.color = color;
    }
    void Update()
    {
        
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
        if (eventData.button == PointerEventData.InputButton.Left )
        {
            if (item != null)
            {
                //아이템 사용함수 작성
                if (item.itemType == Item.ItemType.Countable)
                {
                    SetSlotCount(-1);
                    Debug.Log("아이템 사용 테스트");
                }
            }
            else
            {
                NoticeText.text = "아이템이 없습니다!";
            }
        }
        if (Input.GetKeyDown(ItemSlotnum1))
        {
            if (item != null)
            {
                //아이템 사용함수 작성
                if (item.itemType == Item.ItemType.Countable)
                {
                    Inventory.Instance.slots[0].SetSlotCount(-1);
                    Debug.Log("아이템 사용 테스트");
                }
                else
                {
                    NoticeText.text = "아이템이 없습니다!";
                }
            }
        }
        if (Input.GetKeyDown(ItemSlotnum2))
        {
            if (item != null)
            {
                //아이템 사용함수 작성
                if (item.itemType == Item.ItemType.Countable)
                {
                    Inventory.Instance.slots[1].SetSlotCount(-1);
                    Debug.Log("아이템 사용 테스트");
                }
                else
                {
                    NoticeText.text = "아이템이 없습니다!";
                }
            }
        }
        if (Input.GetKeyDown(ItemSlotnum3))
        {
            if (item != null)
            {
                //아이템 사용함수 작성
                if (item.itemType == Item.ItemType.Countable)
                {
                    Inventory.Instance.slots[2].SetSlotCount(-1);
                    Debug.Log("아이템 사용 테스트");
                }
                else
                {
                    NoticeText.text = "아이템이 없습니다!";
                }
            }
        }
        if (Input.GetKeyDown(ItemSlotnum4))
        {
            if (item != null)
            {
                //아이템 사용함수 작성
                if (item.itemType == Item.ItemType.Countable)
                {
                    Inventory.Instance.slots[3].SetSlotCount(-1);
                    Debug.Log("아이템 사용 테스트");
                }
                else
                {
                    NoticeText.text = "아이템이 없습니다!";
                }
            }
        }
        if (Input.GetKeyDown(ItemSlotnum5))
        {
            if (item != null)
            {
                //아이템 사용함수 작성
                if (item.itemType == Item.ItemType.Countable)
                {
                    Inventory.Instance.slots[4].SetSlotCount(-1);
                    Debug.Log("아이템 사용 테스트");
                }
                else
                {
                    NoticeText.text = "아이템이 없습니다!";
                }
            }
        }
        if (Input.GetKeyDown(ItemSlotnum6))
        {
            if (item != null)
            {
                //아이템 사용함수 작성
                if (item.itemType == Item.ItemType.Countable)
                {
                    Inventory.Instance.slots[5].SetSlotCount(-1);
                    Debug.Log("아이템 사용 테스트");
                }
                else
                {
                    NoticeText.text = "아이템이 없습니다!";
                }
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.gameObject.CompareTag("Item"))
        {
            GButtonImage.transform.gameObject.SetActive(true);
            {
                if(Input.GetKeyDown(KeyCode.G)){
                    var item = other.gameObject.GetComponent<Inventory>();

                    item.Pickup();
                    Destroy(item.gameObject);
                }

            }
           
        }
        else
        {
            GButtonImage.transform.gameObject.SetActive(false);
        }
    }

}
