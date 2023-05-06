//using UnityEngine;

//public class Inventory : MonoBehaviour
//{
//    //임성훈

//    #region 싱글톤

//    public static Inventory Instance; //인벤토리 싱글톤

//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//            DontDestroyOnLoad(gameObject);
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }
    

//    #endregion


//    [Header("슬롯 베이스")] [SerializeField] private GameObject slotsBase; //슬롯들 담겨있는 베이스
//    [Header("Resources에 있는 모든 아이템을 넣어주세요")] [SerializeField] private Item[] baseItemData; //기본 아이템들
    
//    public Slot[] slots; //슬롯들
    
//    void Start()
//    {
//        slots = slotsBase.GetComponentsInChildren<Slot>(); //슬롯 베이스에 있는 모든 슬롯 가져오기
//        InitializeItems();
//    }

//    public void SaveInventory()
//    {
//        //인벤토리 저장 기능
//        //저장할때는 아이템의 개수와 이름만 저장
//    }

//    public void LoadInventory()
//    {
//        //인벤토리 불러오기 기능
        
//        //불러오기에서 슬롯 아이템 이름으로 아이템을 찾고 그 아이템에 맞는 개수와 이미지를 불러옴
//    }

//    private void InitializeItems()
//    {
//        foreach (var item in baseItemData) // 기본 아이템에 대한 동작 설정
//        {
//            item.OnPickUp = () => GetItem(item); //주울 수 있음 기능을 각 아이템에 부여
//        }
//    }

//    private void GetItem(Item _item, int _count = 1) //아이템 획득기능
//    {
//        //쌓을수 있는 아이템일때는 아래 코드 작동
//        if (_item.itemType != Item.ItemType.NoneCountable)
//        {
//            foreach (var slot in slots)
//            {
//                if (slot.item == null || slot.item.itemName != _item.itemName) continue;
//                slot.SetSlotCount(_count);
//                return;
//            }
//        }

//        //쌓을수 없는 아이템일때는 아래 코드 작동
//        foreach (var slot in slots)
//        {
//            if (slot.item != null) continue;
//            slot.AddItem(_item, _count);
//            return;
//        }
//    }

//}
