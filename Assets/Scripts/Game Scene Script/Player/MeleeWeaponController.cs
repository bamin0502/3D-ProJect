using System.Collections;
using System.Collections.Generic;
using mino;
using UnityEngine;

public class MeleeWeaponController : MonoBehaviour
{
    [SerializeField] private WeaponController weaponController;
    [SerializeField] private PlayerMovement playerMovement;

    private bool isAttack = false;
    private RaycastHit hitInfo;
    private Transform currentTarget;
    private Weapon currentWeapon;

    void Update()
    {
        TryAttack();
    }

    private void TryAttack()
    {
        currentWeapon = weaponController.equippedWeapon;
        currentTarget = weaponController.currentTarget;

        if (currentTarget != null && IsTargetInRange())
        {
            if (!isAttack)
            {
                StartCoroutine(AttackCoroutine());
            }
        }
        else
        {
            isAttack = false;
        }
    }

    IEnumerator AttackCoroutine()
    {
        isAttack = true;
        
        playerMovement.ChangedState(PlayerState.HammerAttackIdle);

        while (isAttack)
        {
            switch (currentWeapon.weaponType)
            {
                case WeaponType.Bow:
                    playerMovement.ani.ani.SetTrigger("BowAttack");
                    break;
                case WeaponType.Gun:
                    playerMovement.ani.ani.SetTrigger("GunAttack");
                    break;
                case WeaponType.OneHanded:
                    playerMovement.ani.ani.SetTrigger("OneHandedAttack");
                    break;
                case WeaponType.TwoHanded:
                    playerMovement.ani.ani.SetTrigger("TwoHandedAttack");
                    break;
            }

            if (currentTarget == null ||
                !IsTargetInRange()) // if target disappeared or not in range anymore, stop attack
            {
                isAttack = false;
            }

            yield return new WaitForSeconds(currentWeapon.attackInterval);
        }
    }

    private bool IsTargetInRange()
    {
        return Vector3.Distance(transform.position, currentTarget.position) <= currentWeapon.range;
    }
}
