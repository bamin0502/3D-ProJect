using System.Collections;
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

        if (currentTarget == null)
        {
            _isAttack = false;
        }

        if (!_weaponController.isAttack && attackRoutine != null)
        {
            _isAttack = false;
            StopCoroutine(attackRoutine);
            return;
        }

        if (_weaponController.isAttack && !_isAttack && currentTarget != null)
        {
            attackRoutine = StartCoroutine(AttackCoroutine());
        }
    }
    private IEnumerator AttackCoroutine()
    {
        if (MultiScene.Instance.currentUser != transform.gameObject.name) yield break;
        
        _isAttack = true;

        _playerMovement.ChangedState(PlayerState.Idle);
        MultiScene.Instance.BroadCastingAnimation((int)PlayerState.Idle);

        var weaponInterval = currentWeapon.attackInterval;
        var weaponType = currentWeapon.weaponType;

        while (true)
        {
            if(!_isAttack) yield break;
            
            switch (weaponType)
            {
                case WeaponType.Bow:
                    _playerMovement.SetAnimationTrigger(BowAttack);
                    MultiScene.Instance.BroadCastingAnimation(BowAttack, true);
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

    public void ShotBow()
    {
        var currentBow = currentWeapon.GetComponent<RangedWeapon>();
        var transform1 = currentBow.transform;
        var arrow = Instantiate(currentBow.projectilePrefab, transform1.position, transform1.rotation);
        arrow.TryGetComponent(out Projectile arrow1);
        arrow1.damage = currentBow.GetDamage();
        arrow1.Shot(currentTarget);
    }

    public void BeginShot()
    {
        
    }
}
