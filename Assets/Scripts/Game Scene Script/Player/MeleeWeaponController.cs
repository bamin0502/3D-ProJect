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

        if (!weaponController.isAttack && attackRoutine != null)
        {
            isAttack = false;
            StopCoroutine(attackRoutine);
        }

        if (weaponController.isAttack && !isAttack)
        {
            attackRoutine = StartCoroutine(AttackCoroutine());
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
                    StartCoroutine(ArrowSpawnCoroutine());
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
            }

            yield return new WaitForSeconds(weaponInterval);
        }
    }

    private IEnumerator ArrowSpawnCoroutine(){
        yield return new WaitForSeconds(0.1f);
        SoundManager.instance.PlaySE("Bow_String");
        var currentBow = currentWeapon.GetComponent<RangedWeapon>();
        var arrow = Instantiate(currentBow.projectilePrefab, currentBow.arrowPos.position, Quaternion.LookRotation(currentBow.arrowPos.forward));
        arrow.transform.parent = currentBow.transform;
        StartCoroutine(arrow.GetComponent<Projectile>().ShotCoroutine(currentTarget));
    }
}
