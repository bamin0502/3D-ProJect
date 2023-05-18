using UnityEngine;
using UnityEngine.AI;

public class WeaponController : MonoBehaviour
{
    //임성훈

    public Transform weaponHolder;
    public NavMeshAgent agent;
    public float weaponPickupRange = 1.5f;
    private Weapon targetedWeapon;
    public Weapon equippedWeapon;
    public Transform currentTarget;
    private float attackTimer;
    //새로운 거 
    public Vector3 PickPosition;
    public Vector3 PickRotation;
    //김하겸
    //private Weapon targetedWeapon;
    //private Weapon equippedWeapon;
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
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    SetTarget(hit.transform);
                }
                else if (hit.collider.TryGetComponent(out targetedWeapon))
                {
                    
                }
                else
                {
                    ClearTarget();
                }
            }
        }
    }

    private void SetTarget(Transform target)
    {
        currentTarget = target;
        agent.stoppingDistance = GetWeaponRange();
        attackTimer = 0f;
    }

    private float GetWeaponRange()
    {
        return equippedWeapon is RangedWeapon weapon ? weapon.range : ((MeleeWeapon)equippedWeapon).range;
    }

    private void ClearTarget()
    {
        currentTarget = null;
        agent.stoppingDistance = 0f;
    }

    private void TryPickupWeapon()
    {
        float distance = Vector3.Distance(transform.position, targetedWeapon.transform.position);
        if (distance <= weaponPickupRange)
        {
            EquipWeapon(targetedWeapon);
            targetedWeapon = null;
        }
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

        newWeapon.isEquipped = true;
        newWeapon.transform.parent = weaponHolder;
        //새로운 코드 
        newWeapon.transform.localPosition = PickPosition;
        newWeapon.transform.localRotation = Quaternion.Euler(PickRotation);
        //기존 코드
        //newWeapon.transform.localPosition = Vector3.zero;
        //newWeapon.transform.localRotation = Quaternion.identity;
        //칼 같은 경우는 한손으로도 어색하지가 않아서 이리 해도 되지만
        //총 같은 경우는 한손이면 어색한 것이 좀 있음 
        //이 같은 경우는 총드는 애니메이션을 찾은다음 
        //그 총들었을때의 애니메이션을 실행시키는 방식으로 하는것이 좋습니다.
        //저도 일단 찾아볼테니까 보고 참고만 해주세요 기존에 하고 있던 방식을
        //굳히 바꿀 필요는 없습니다.
        equippedWeapon = newWeapon;
    }

    private void DropEquippedWeapon()
    {
        equippedWeapon.transform.SetParent(null);
        equippedWeapon.isEquipped = false;
        equippedWeapon.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        equippedWeapon.transform.position = transform.position + transform.forward * 1f;
        equippedWeapon = null;
    }
}
        
    
