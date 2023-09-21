using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class WeaponController : MonoBehaviour
{
    //임성훈
    [SerializeField] private PlayerMovement playerMovement;
    public Transform weaponHolder;
    public Transform weaponHolder2;
    public NavMeshAgent agent;
    private readonly float weaponPickupRange = 2f;
    private Weapon targetedWeapon;
    public Weapon equippedWeapon;
    public Transform currentTarget;
    private float attackTimer;
    public bool isAttack;
    public Canvas IconCanvas;
    [SerializeField]
    public TMP_Text WeaponText;
    private static readonly int Pickup = Animator.StringToHash("ItemPickup");
    private bool isInRangeToPickup = false;
    private bool isPickingUpWeapon = false;
    private void Update()
    {
        HandleInput();

        if (targetedWeapon != null)
        {
            TryPickupWeapon();
        }

        if (currentTarget != null)
        {
            MoveTowardsTarget();
            AttackTarget();
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

    private void Start()
    {
        StartCoroutine(CheckAttackCoroutine());
    }

    private void OnDrawGizmos()
    {
        #if UNITY_EDITOR
        // 무기 주울 수 있는 범위를 시각적으로 표시합니다.
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, weaponPickupRange);
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
        WeaponText.gameObject.SetActive(true);
        WeaponText.text = "무기 줍기" + "<color=yellow>" + "(G)" + "</color>";
    }

    private void HidePickupText()
    {
        WeaponText.text = "";
        WeaponText.gameObject.SetActive(false);
    }
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            TryPickupWeapon();
        }
        // 오른쪽 마우스 클릭을 확인합니다.
        if (Input.GetMouseButtonDown(1))
        {
            if (Camera.main != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                LayerMask layerMask = ~LayerMask.GetMask("Player");

                if (Physics.Raycast(ray, out var hit, Mathf.Infinity, layerMask))
                {
                    if (hit.collider.CompareTag("Enemy"))
                    {
                        // 오른쪽 마우스 클릭 시 타겟을 설정합니다.
                        SetTarget(hit.transform);
                    }
                }
            }
        }
    }

    private void SetTarget(Transform target)
    {
        if(equippedWeapon == null) return;
        currentTarget = target;
        agent.stoppingDistance = GetWeaponRange();
        attackTimer = 0f;
    }

    private IEnumerator CheckAttackCoroutine()
    {
        while (true)
        {
            if (currentTarget == null || equippedWeapon == null)
            {
                isAttack = false;
            }
            else
            {
                var range = GetWeaponRange();
                float distance = Vector3.Distance(transform.position, currentTarget.position);
                isAttack = distance <= range;
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    private float GetWeaponRange()
    {
        if (equippedWeapon == null) return 0;
        return equippedWeapon is RangedWeapon weapon ? weapon.range : ((MeleeWeapon)equippedWeapon).range;
    }

    private void ClearTarget()
    {
        currentTarget = null;
        isAttack = false;
        agent.stoppingDistance = 0f;
    }

    private void TryPickupWeapon()
    {
        //무기 콜라이더 배열 
        Collider[] itemColliders = new Collider[1];

        int weaponCount = Physics.OverlapSphereNonAlloc(transform.position, weaponPickupRange, itemColliders);
        
        //만약 무기를 찾았다면
        if (weaponCount > 0)
        {
            for (int i = 0; i < weaponCount; i++)
            {
                Collider collider = itemColliders[i];
               
                if (collider.CompareTag("Weapon"))
                {
                    playerMovement.ani.ani.SetTrigger(Pickup);
                    StartCoroutine(EquipWeaponAfterDelay(collider.GetComponent<Weapon>(), 0.1f));
                    HidePickupText();
                }
            }
        }
        
    }

    private IEnumerator EquipWeaponAfterDelay(Weapon newWeapon, float delay)
    {
        yield return new WaitForSeconds(delay);
        EquipWeapon(newWeapon);
        targetedWeapon = null;
        isPickingUpWeapon = false;
        HidePickupText();
    }
    
    private void MoveTowardsTarget()
    {
        agent.SetDestination(currentTarget.position);
    }

    private void AttackTarget()
    {
        if (Vector3.Distance(transform.position, currentTarget.position) <= agent.stoppingDistance)
        {
            FaceTarget();
        }
    }

    private void FaceTarget()
    {
        Quaternion targetRotation = Quaternion.LookRotation(currentTarget.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
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
        
    
