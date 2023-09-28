using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class Multi_ItemDropController : MonoBehaviour
{
    [SerializeField]
    private float pickupRadius;  // 아이템 습득이 가능한 최대 반경
    private static readonly int Pickup = Animator.StringToHash("ItemPickup");
    private bool pickupActivated = false;  // 아이템 습득 가능할 시 True 
    private MultiPlayerMovement _playerMovement;
    private Collider[] itemColliders;  // 아이템 콜라이더 배열
    
    
    [SerializeField]
    private LayerMask layerMask;  // 특정 레이어를 가진 오브젝트에 대해서만 습득할 수 있어야 합니다.
    
    [SerializeField]
    private Multi_Inventory theInventory;
    private void Awake()
    {
        theInventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Multi_Inventory>();
        pickupRadius = 3;
    }
    private void Start()
    {
    }
    void FixedUpdate()
    {
        if(MultiScene.Instance.currentUser != transform.gameObject.name) return;
        CheckItem();
        TryAction();
    }

    private void TryAction()
    {
        if (pickupActivated && Input.GetKeyDown(KeyCode.G))
        {
            PickUpItem();
        }
    }

    private void CheckItem()
    {
        // 아이템 콜라이더 배열에 충돌된 콜라이더를 넣어줍니다.
        itemColliders = new Collider[10];  // 적절한 크기로 조정하세요.

        // 메모리 할당 없이 아이템을 감지합니다.
        int itemCount = Physics.OverlapSphereNonAlloc(transform.position, pickupRadius, itemColliders, layerMask);

        if (itemCount > 0)
        {
            for (int i = 0; i < itemCount; i++)
            {
                Collider collider=itemColliders[i];
                if (collider.CompareTag("Item"))
                {
                    ItemInfoAppear();
                    return; // 가장 가까운 아이템만 처리하도록 리턴
                }
            }
        }
        else
        {
            itemColliders = null;
            ItemInfoDisappear();
        }
    }

    private void ItemInfoAppear()
    {
        pickupActivated = true;
        
        string itemName = itemColliders[0].GetComponent<ItemPickup>().item.itemName;
        MultiScene.Instance.noticeText.gameObject.SetActive(true);
        MultiScene.Instance.noticeText.text = itemName+" 아이템 획득 " + "<color=yellow>" + "(G)" + "</color>";
    }

    private void ItemInfoDisappear()
    {
        pickupActivated = false;
        MultiScene.Instance.noticeText.gameObject.SetActive(false);
    }

    private void PickUpItem()
    {
        if (pickupActivated && itemColliders != null)
        {
            List<GameObject> itemsToDestroy=new List<GameObject>();
            foreach (var t in itemColliders)
            {
                if (t != null && t.CompareTag("Item"))
                {
                    _playerMovement.SetAnimationTrigger(Pickup);
                    MultiScene.Instance.BroadCastingAnimation(Pickup,true);
                    
                    int index = MultiScene.Instance.itemsList.IndexOf(t.gameObject);
                    
                    if(index>=0 &&index<MultiScene.Instance.itemsList.Count)
                    {
                       
                        itemsToDestroy.Add(t.gameObject);
                    }
                    MultiScene.Instance.BroadCastingPickItem(index);
                    SoundManager.instance.PlaySE("Item Drop");
                }
            }
            foreach (var itemToDestroy in itemsToDestroy)
            {
                Destroy(itemToDestroy);
            }
        }
    }
    private void OnDrawGizmos()
    {
        #if UNITY_EDITOR    
        // 현재 설정된 layerMask 값을 시각적으로 표시합니다.
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
        #endif
    }

}
