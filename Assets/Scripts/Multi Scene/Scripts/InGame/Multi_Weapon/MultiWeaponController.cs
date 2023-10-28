using System.Collections;
using UnityEngine;
using UnityEngine.AI;
#if UNITY_EDITOR
using UnityEditor;
#endif


[RequireComponent(typeof(MultiPlayerMovement))]
public class MultiWeaponController : MonoBehaviour
{
    //임성훈
    
    private static readonly int Pickup = Animator.StringToHash("ItemPickup");
    private MultiPlayerSkill _playerSkill;
    private const float WeaponPickupRange = 2f;
    
    private MultiPlayerMovement _playerMovement;
    private NavMeshAgent _agent;
    
    public Transform weaponHolder;
    public Transform weaponHolder2;
    
    private Weapon targetedWeapon;
    public Weapon equippedWeapon;
    public Transform currentTarget;
    private float attackTimer;
    public bool isAttack;
    public Canvas IconCanvas;
    
    private bool isInRangeToPickup = false;
    private bool isPickingUpWeapon = false;

    private void Start()
    {
        _playerSkill = GetComponent<MultiPlayerSkill>();
        _playerMovement = GetComponent<MultiPlayerMovement>();
        _agent = _playerMovement.navAgent;
    }
    
    private void Update()
    {
        if (currentTarget != null)
        {
            AttackTarget();
        }
        
        if(MultiScene.Instance.currentUser != transform.gameObject.name) return;
        
        HandleInput();

        if (targetedWeapon != null)
        {
            TryPickupWeapon();
        }
        

        if (!isPickingUpWeapon && isInRangeToPickup)
        {
            ShowPickupText();
        }
        else
        {
            HidePickupText();
        }
    }

    

    private void OnDrawGizmos()
    {
        #if UNITY_EDITOR
        // 무기 주울 수 있는 범위를 시각적으로 표시합니다.
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, WeaponPickupRange);
        #endif
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            isInRangeToPickup = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            isInRangeToPickup = false;
            HidePickupText();
        }
    }
    private void ShowPickupText()
    {
        MultiScene.Instance.noticeText.gameObject.SetActive(true);
        MultiScene.Instance.noticeText.text = "무기 줍기" + "<color=yellow>" + "(G)" + "</color>";
    }

    private void HidePickupText()
    {
        MultiScene.Instance.noticeText.text = "";
        MultiScene.Instance.noticeText.gameObject.SetActive(false);
    }
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            TryPickupWeapon();
        }
    }

    public void SetTarget(int enemy)
    {
        if (enemy >= 0 && enemy < MultiScene.Instance.enemyList.Count)
        {
            var target = MultiScene.Instance.enemyList[enemy];

            if (target != null)
            {
                if(equippedWeapon == null) return;
                currentTarget = target.transform;
                _agent.stoppingDistance = GetWeaponRange();
                var position = currentTarget.position;
                _agent.SetDestination(position);
                attackTimer = 0f;
        
                var range = GetWeaponRange();
                float distance = Vector3.Distance(transform.position, position);
                isAttack = distance <= range;
            }
            else
            {
                Debug.LogWarning("Enemy 컴포넌트를 찾을 수 없습니다. GameObject 이름: " + target.name);
            }
        }
        else
        {
            Debug.LogWarning("적의 인덱스가 범위를 벗어났습니다. 인덱스: " + enemy);
        }
    }

    private float GetWeaponRange()
    {
        if (equippedWeapon == null) return 0;
        return equippedWeapon is RangedWeapon weapon ? weapon.range : ((MeleeWeapon)equippedWeapon).range;
    }

    public void ClearTarget()
    {
        currentTarget = null;
        isAttack = false;
        _agent.stoppingDistance = 0f;
    }

    private void TryPickupWeapon()
    {
        //무기 콜라이더 배열 
        Collider[] itemColliders = new Collider[1];

        int weaponCount = Physics.OverlapSphereNonAlloc(transform.position, WeaponPickupRange, itemColliders);
        
        //만약 무기를 찾았다면
        if (weaponCount > 0)
        {
            for (int i = 0; i < weaponCount; i++)
            {
                Collider collider = itemColliders[i];
               
                if (collider.CompareTag("Weapon"))
                {
                    _playerMovement.SetAnimationTrigger(Pickup);
                    MultiScene.Instance.BroadCastingAnimation(Pickup, true);

                    int index = MultiScene.Instance.weaponsList.IndexOf(collider.gameObject);

                    PickWeapon(index);
                    _playerSkill.ChangeWeapon(index);
                    MultiScene.Instance.BroadCastingPickWeapon(index);
                    
                    HidePickupText();
                }
            }
        }
    }

    public void PickWeapon(int index)
    {
        Weapon obj = MultiScene.Instance.weaponsList[index].GetComponent<Weapon>();
        
        
        
        
        StartCoroutine(EquipWeaponAfterDelay(obj, 0.1f));
    }

    private IEnumerator EquipWeaponAfterDelay(Weapon newWeapon, float delay)
    {
        yield return new WaitForSeconds(delay);
        EquipWeapon(newWeapon);
        targetedWeapon = null;
        isPickingUpWeapon = false;
        HidePickupText();
    }

    private void AttackTarget()
    {
        if (Vector3.Distance(transform.position, currentTarget.position) <= _agent.stoppingDistance)
        {
            Quaternion targetRotation = Quaternion.LookRotation(currentTarget.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }
    
    private void EquipWeapon(Weapon newWeapon)
    {
        if (equippedWeapon != null)
        {            
            DropEquippedWeapon();
        }
        if(newWeapon != null)
        {
            newWeapon.isEquipped = true;
            if(newWeapon.TryGetComponent(out Collider collider))
            {
                collider.enabled = false;
            }

            var transform1 = newWeapon.transform;
            transform1.parent = newWeapon.weaponType != WeaponType.Bow ? weaponHolder : weaponHolder2;
            transform1.localPosition = newWeapon.PickPosition;
            newWeapon.transform.localRotation = Quaternion.Euler(newWeapon.PickRotation);
            equippedWeapon = newWeapon;
            if (equippedWeapon.iconCanvas != null)
            {
                DisableCanvas(equippedWeapon.iconCanvas); // 현재 무기의 아이콘 비활성화
            }
        }

       
    }

    private void DropEquippedWeapon()
    {

        if(equippedWeapon != null)
        {
            if(equippedWeapon.iconCanvas != null)
            {
                EnableCanvas(equippedWeapon.iconCanvas);
            }
            equippedWeapon.transform.SetParent(null);
            equippedWeapon.isEquipped = false;
            if(equippedWeapon.TryGetComponent(out Collider collider)){
                collider.enabled = true;
            }

            Transform transform1;
            (transform1 = equippedWeapon.transform).rotation = Quaternion.Euler(90f, 0f, 0f);
            var transform2 = transform;
            transform1.position = transform2.position + transform2.forward * 1f;
            equippedWeapon = null;
        }

    }
    private void DisableCanvas(Canvas canvas)
    {
        canvas.enabled = false;
    }

    private void EnableCanvas(Canvas canvas)
    {
        canvas.enabled = true;
    }
}
        
    
