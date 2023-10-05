using System;
using System.Collections;
using System.Collections.Generic;
using mino;
using UnityEngine;

public class MultiMeleeWeaponController : MonoBehaviour
{
    private MultiWeaponController _weaponController;
    private MultiPlayerMovement _playerMovement;

    private bool _isAttack;
    private RaycastHit hitInfo;
    private Transform currentTarget;
    private Weapon currentWeapon;
    private Coroutine attackRoutine;
    private static readonly int BowAttack = Animator.StringToHash("BowAttack");
    private static readonly int OneHandedAttack = Animator.StringToHash("OneHandedAttack");
    private static readonly int TwoHandedAttack = Animator.StringToHash("TwoHandedAttack");
    private void Start()
    {
        _weaponController = GetComponent<MultiWeaponController>();
        _playerMovement = GetComponent<MultiPlayerMovement>();
    }
    void Update()
    {
        TryAttack();
    }
    private void TryAttack()
    {
        currentWeapon = _weaponController.equippedWeapon;
        currentTarget = _weaponController.currentTarget;

        if (currentWeapon == null) return;

        if (!_weaponController.isAttack && attackRoutine != null)
        {
            _isAttack = false;
            StopCoroutine(attackRoutine);
        }

        if (_weaponController.isAttack && !_isAttack)
        {
            attackRoutine = StartCoroutine(AttackCoroutine());
        }
    }
    private IEnumerator AttackCoroutine()
    {
        if (MultiScene.Instance.currentUser != transform.gameObject.name) yield break;
        
        _isAttack = true;

        _playerMovement.ChangedState(PlayerState.HammerAttackIdle);
        MultiScene.Instance.BroadCastingAnimation((int)PlayerState.HammerAttackIdle);

        var weaponInterval = currentWeapon.attackInterval;
        var weaponType = currentWeapon.weaponType;

        while (_isAttack)
        {
            switch (weaponType)
            {
                case WeaponType.Bow:
                    _playerMovement.SetAnimationTrigger(BowAttack);
                    MultiScene.Instance.BroadCastingAnimation(BowAttack, true);
                    CoroutineArrow();
                    break;
                case WeaponType.OneHanded:
                    _playerMovement.SetAnimationTrigger(OneHandedAttack);
                    MultiScene.Instance.BroadCastingAnimation(OneHandedAttack, true);
                    break;
                case WeaponType.TwoHanded:
                    _playerMovement.SetAnimationTrigger(TwoHandedAttack);
                    MultiScene.Instance.BroadCastingAnimation(TwoHandedAttack, true);
                    break;
                default:
                    break;
            }

            yield return new WaitForSeconds(weaponInterval);
        }
    }

    public void CoroutineArrow()
    {
        StartCoroutine(ArrowSpawnCoroutine());
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
