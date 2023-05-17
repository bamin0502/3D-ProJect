using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ItemDropController : MonoBehaviour
{
    [SerializeField]
    private float range;  // 아이템 습득이 가능한 최대 거리

    private bool pickupActivated = false;  // 아이템 습득 가능할시 True 

    private RaycastHit hitInfo;  // 충돌체 정보 저장

    [SerializeField]
    private LayerMask layerMask;  // 특정 레이어를 가진 오브젝트에 대해서만 습득할 수 있어야 한다.

    [SerializeField]
    private TMP_Text actionText;  // 행동을 보여 줄 텍스트

    void FixedUpdate()
    {
        CheckItem();
        TryAction();
        OnDrawGizmos();
    }

    private void TryAction()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            CheckItem();
            CanPickUp();
        }
    }

    private void CheckItem()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, range, layerMask))
        {
            if (hitInfo.transform.tag == "Item")
            {
                ItemInfoAppear();
            }
        }
        else
            ItemInfoDisappear();
    }

    private void ItemInfoAppear()
    {
        pickupActivated = true;
        actionText.gameObject.SetActive(true);
        actionText.text = hitInfo.transform.GetComponent<ItemPickup>().item.itemName + " 획득 " + "<color=yellow>" + "(G)" + "</color>";
    }

    private void ItemInfoDisappear()
    {
        pickupActivated = false;
        actionText.gameObject.SetActive(false);
    }

    private void CanPickUp()
    {
        if (pickupActivated)
        {
            if (hitInfo.transform != null)
            {
                Debug.Log(hitInfo.transform.GetComponent<ItemPickup>().item.itemName + " 획득 했습니다.");  // 인벤토리 넣기
                Destroy(hitInfo.transform.gameObject);
                ItemInfoDisappear();
            }
        }
    }
    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        // 현재 설정된 layerMask 값을 시각적으로 표시합니다.
        Handles.Label(transform.position, "Layer Mask: " + LayerMask.LayerToName(layerMask));

        // 정면으로 레이를 그려서 레이어가 앞으로 나가는지 여부를 확인합니다.
        Debug.DrawRay(transform.position, transform.forward * range, Color.red);
#endif
    }
}