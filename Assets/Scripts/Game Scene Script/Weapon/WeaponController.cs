using UnityEngine;
using UnityEngine.AI;

public class WeaponController : MonoBehaviour
{
    //임성훈
    
    public Transform weaponHolder;
    public NavMeshAgent agent;
    public float weaponPickupRange = 1.5f;
    private Weapon targetedWeapon;
    private Weapon equippedWeapon;
    private Transform currentTarget;
    private float attackTimer;

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
            UpdateAttackTimerAndAttack();
        }
    }

    private void FaceTarget()
    {
        Quaternion targetRotation = Quaternion.LookRotation(currentTarget.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }

    private void UpdateAttackTimerAndAttack()
    {
        attackTimer += Time.deltaTime;
        if (attackTimer >= equippedWeapon.attackInterval)
        {
            equippedWeapon.Attack(currentTarget);
            attackTimer = 0f;
        }
    }

    private void EquipWeapon(Weapon newWeapon)
    {
        if (equippedWeapon != null)
        {
            DropEquippedWeapon();
        }

        newWeapon.isEquipped = true;
        newWeapon.transform.parent = weaponHolder;
        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localRotation = Quaternion.identity;

        equippedWeapon = newWeapon;
    }

    private void DropEquippedWeapon()
    {
        equippedWeapon.transform.SetParent(null);
        equippedWeapon.isEquipped = false;
        equippedWeapon.transform.position = transform.position + transform.forward * 1f;
        equippedWeapon = null;
    }
}
        
    
