using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class Inventory : SerializedMonoBehaviour
{

    // 공개
    public List<GameObject> AllSlot;    // 모든 슬롯을 관리해줄 리스트.
    public RectTransform InvenRect;     // 인벤토리의 Rect
    public GameObject OriginSlot;       // 오리지널 슬롯.
    public List<Item> items=new List<Item>();
    public float slotSize;              // 슬롯의 사이즈.
    public float slotGap;               // 슬롯간 간격.
    public float slotCountX;            // 슬롯의 가로 개수.
    public float slotCountY;            // 슬롯의 세로 개수.

    // 비공개.
    private float InvenWidth;           // 인벤토리 가로길이.
    private float InvenHeight;          // 인밴토리 세로길이.
    private int slotMax;            // 빈 슬롯의 개수.

    void Awake()
    {
        AllSlot = new List<GameObject>();
        // 인벤토리 이미지의 가로, 세로 사이즈 셋팅.

        InvenWidth = (slotCountX * slotSize) + (slotCountX - 1) * slotGap;
        InvenHeight = (slotCountY * slotSize) + (slotCountY - 1) * slotGap;

        // 인벤토리의 크기 설정
        InvenRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, InvenWidth);
        InvenRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, InvenHeight);

        slotMax = (int)(slotCountX * slotCountY);    // 슬롯의 최대 개수 계산
        if (OriginSlot == null)
        {
            Debug.LogError("OriginSlot이 할당되지 않았습니다.");
            return;
        }
        // 슬롯 생성
        for (int i = 0; i < slotMax; i++)
        {
            // 오리지널 슬롯 복사
            GameObject slot = Instantiate(OriginSlot);
            slot.name = "slot_" + i.ToString();
            slot.transform.SetParent(transform);    // 슬롯의 부모를 인벤토리로 설정
            slot.transform.localScale = Vector3.one;
            slot.GetComponent<RectTransform>().localPosition = GetSlotPosition(i); // 슬롯의 위치 설정

            AllSlot.Add(slot);  // 생성한 슬롯을 리스트에 추가
        }
    }
    // 슬롯의 위치를 계산하는 메서드
    private Vector3 GetSlotPosition(int index)
    {
    float posX = (index % slotCountX) * (slotSize + slotGap);
    float posY = -(index / slotCountX) * (slotSize + slotGap);

    return new Vector3(posX, posY, 0f);
    }
    // 아이템을 넣기위해 모든 슬롯을 검사.
    public bool AddItem(Item items)
    {
        // 슬롯에 추가할 수 있는 빈 슬롯을 찾음
        GameObject emptySlot = FindEmptySlot();
        if (emptySlot != null)
        {
            // 빈 슬롯이 있을 경우 아이템을 해당 슬롯에 추가하고 성공을 반환
            Slot slot = emptySlot.GetComponent<Slot>();
            slot.AddItem(items);
            return true;
        }
        else
        {
            // 빈 슬롯이 없을 경우 아이템 추가 실패를 반환
            return false;
        }

    }
    // 빈 슬롯을 찾는 메서드
    private GameObject FindEmptySlot()
    {
        if (AllSlot.Count == 0)
        {
            Debug.LogError("AllSlot이 비어있습니다. 슬롯을 생성한 후에 아이템을 추가하세요.");
            return null;
        }
        // 모든 슬롯을 순회하며 빈 슬롯을 찾음
        foreach (GameObject slot in AllSlot)
        {
            Slot slotComponent = slot.GetComponent<Slot>();
            if (slotComponent.IsEmpty())
            {
                return slot;
            }
        }

        // 빈 슬롯이 없을 경우 null을 반환
        return null;
    }
    public Slot NearDisSlot(Vector3 Pos)
    {
        float Min = 10000f;
        int Index = -1;

        int Count = AllSlot.Count;
        for (int i = 0; i < Count; i++)
        {
            Vector2 sPos = AllSlot[i].transform.GetChild(0).position;
            float Dis = Vector2.Distance(sPos, Pos);

            if (Dis < Min)
            {
                Min = Dis;
                Index = i;
            }
        }

        if (Min > slotSize)
            return null;

        return AllSlot[Index].GetComponent<Slot>();
    }
    // 아이템 옮기기 및 교환.
    public void Swap(Slot slot, Vector3 Pos)
    {
        Slot FirstSlot = NearDisSlot(Pos);

        // 현재 슬롯과 옮기려는 슬롯이 같으면 함수 종료.
        if (slot == FirstSlot || FirstSlot == null)
        {
            slot.UpdateInfo(true, slot.slot.Peek().DefaultImg);
            return;
        }

        // 가까운 슬롯이 비어있으면 옮기기.
        if (!FirstSlot.isSlots())
        {
            Swap(FirstSlot, slot);
        }
        // 교환.
        else
        {
            int Count = slot.slot.Count;
            Item item = slot.slot.Peek();
            Stack<Item> temp = new Stack<Item>();

            {
                for (int i = 0; i < Count; i++)
                    temp.Push(item);

                slot.slot.Clear();
            }

            Swap(slot, FirstSlot);

            {
                Count = temp.Count;
                item = temp.Peek();

                for (int i = 0; i < Count; i++)
                    FirstSlot.slot.Push(item);

                FirstSlot.UpdateInfo(true, temp.Peek().DefaultImg);
            }
        }
    }

    // 1: 비어있는 슬롯, 2: 안 비어있는 슬롯.
    void Swap(Slot xFirst, Slot oSecond)
    {
        int Count = oSecond.slot.Count;
        Item item = oSecond.slot.Peek();

        for (int i = 0; i < Count; i++)
        {
            if (xFirst != null)
                xFirst.slot.Push(item);
        }

        if (xFirst != null)
            xFirst.UpdateInfo(true, oSecond.ItemReturn().DefaultImg);

        oSecond.slot.Clear();
        oSecond.UpdateInfo(false, oSecond.DefaultImg);
    }
}
