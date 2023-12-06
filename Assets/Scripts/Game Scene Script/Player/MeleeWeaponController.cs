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
    private static readonly int BowAttack = Animator.StringToHash("BowAttack");
    private static readonly int GunAttack = Animator.StringToHash("GunAttack");
    private static readonly int OneHandedAttack = Animator.StringToHash("OneHandedAttack");
    private static readonly int TwoHandedAttack = Animator.StringToHash("TwoHandedAttack");

    void Update()
    {
        TryAttack();
    }

    private void TryAttack()
    {
        currentWeapon = weaponController.equippedWeapon;
        currentTarget = weaponController.currentTarget;

        if (currentWeapon == null) return;

        switch (weaponController.isAttack)
        {
            case false when attackRoutine != null:
                isAttack = false;
                StopCoroutine(attackRoutine);
                break;
            case true when !isAttack:
                attackRoutine = StartCoroutine(AttackCoroutine());
                break;
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
            switch (weaponType)
            {
                case WeaponType.Bow:
                    playerMovement.ani.ani.SetTrigger(BowAttack);
                    break;
                case WeaponType.Gun:
                    playerMovement.ani.ani.SetTrigger(GunAttack);
                    break;
                case WeaponType.OneHanded:
                    playerMovement.ani.ani.SetTrigger(OneHandedAttack);
                    break;
                case WeaponType.TwoHanded:
                    playerMovement.ani.ani.SetTrigger(TwoHandedAttack);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            yield return new WaitForSeconds(weaponInterval);
        }
    }

}
