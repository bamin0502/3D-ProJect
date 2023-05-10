using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    //임성훈

    [Header("Resource에서 맞는 아이템 찾아서 넣어주세요")]
    public Item item; // 아이템 정보

    public void PickUp()
    {
        item.OnPickUp?.Invoke(); // 아이템 동작을 처리하는 이벤트를 호출
    }
}
