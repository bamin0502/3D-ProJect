using System;
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
    private Coroutine attackRoutine;

    void Update()
    {
        TryAttack();
    }

    private void TryAttack()
    {
        currentWeapon = weaponController.equippedWeapon;
        currentTarget = weaponController.currentTarget;
        
        if(currentWeapon == null) return;
        
        if (currentTarget != null && IsTargetInRange())
        {
            if (!isAttack)
            {
                attackRoutine = StartCoroutine(AttackCoroutine());
            }
        }
    }

    private IEnumerator AttackCoroutine()
    {
        isAttack = true;
        
        playerMovement.ChangedState(PlayerState.HammerAttackIdle);

        var weaponInterval = currentWeapon.attackInterval;
        var weaponType = currentWeapon.weaponType;

        while (isAttack)
        {
            if (currentTarget == null || !IsTargetInRange())
            {
                isAttack = false;
                StopCoroutine(attackRoutine);
            }

            switch (weaponType)
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
            
            yield return new WaitForSeconds(weaponInterval);
        }
    }

    private bool IsTargetInRange()
    {
        return Vector3.Distance(transform.position, currentTarget.position) <= currentWeapon.range;
    }
}
