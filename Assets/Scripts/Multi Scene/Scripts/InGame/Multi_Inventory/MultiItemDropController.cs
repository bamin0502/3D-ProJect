using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class MultiItemDropController : MonoBehaviour
{
    private static readonly int Pickup = Animator.StringToHash("ItemPickup");
    [SerializeField] private float pickupRadius;  // 아이템 습득이 가능한 최대 반경
    private bool pickupActivated;  // 아이템 습득 가능할 시 True 
    private Collider[] itemColliders;  // 아이템 콜라이더 배열
    [SerializeField] private LayerMask layerMask;  // 특정 레이어를 가진 오브젝트에 대해서만 습득할 수 있어야 합니다.
    [HideInInspector] public TextMeshProUGUI actionText;  // 행동을 보여 줄 텍스트
    [HideInInspector] public Inventory inventory;
    private MultiPlayerMovement _playerMovement;
    

    private void Start()
    {
        _playerMovement = GetComponent<MultiPlayerMovement>();
    }

    void FixedUpdate()
    {
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
        itemColliders = new Collider[10];  // 테스트 하면서 적절한 크기로 조정하세요.

        // 메모리 할당 없이 아이템을 감지합니다.
        int itemCount = Physics.OverlapSphereNonAlloc(transform.position, pickupRadius, itemColliders, layerMask);

        if (itemCount > 0)
        {
            for (int i = 0; i < itemCount; i++)
            {
                if (itemColliders[i] != null && itemColliders[i].CompareTag("Item"))
                {
                    ItemInfoAppear();
                    return; // 가장 가까운 아이템만 처리하도록 리턴
                }
            }
        }
        else
        {
            ItemInfoDisappear();
        }
    }

    private void ItemInfoAppear()
    {
        pickupActivated = true;

        if (itemColliders[0] != null && itemColliders[0].GetComponent<ItemPickup>() != null && actionText != null)
        {
            string itemName = itemColliders[0].GetComponent<ItemPickup>().item.itemName;
            actionText.text = itemName + " 아이템 획득 " + "<color=yellow>" + "(G)" + "</color>";
        }
        
    }
    private void ItemInfoDisappear()
    {
        pickupActivated = false;
        if (actionText != null)
        {
            actionText.text = "";
            //actionText.gameObject.SetActive(false);
        }
    }

    private void PickUpItem()
    {
        if (pickupActivated && itemColliders != null)
        {
            foreach (var t in itemColliders)
            {
                if (t != null && t.CompareTag("Item") && t.GetComponent<ItemPickup>() != null)
                {
                    int index = MultiScene.Instance.itemsList.IndexOf(t.gameObject);
                
                    if(_playerMovement != null)
                    {
                        _playerMovement.SetAnimationTrigger(Pickup);
                    }

                    MultiScene.Instance.BroadCastingAnimation(Pickup, true);
                
                    if (inventory != null)
                    {
                        inventory.AcquireItem(t.GetComponent<ItemPickup>().item);
                    }
                    Destroy(t.gameObject);
                    MultiScene.Instance.BroadCastingPickItem(index);
                    SoundManager.instance.PlaySE("Item Drop");
                
                    ItemInfoDisappear();
                }
            }
        }
    }
}
