using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MultiPlayerSkill : MonoBehaviour
{
    //플레이어 스킬 관련
    private static readonly int BowSkill = Animator.StringToHash("BowSkill");
    private static readonly int OneHandedSkill = Animator.StringToHash("OneHandedSkill");
    private static readonly int TwoHandedSkill = Animator.StringToHash("TwoHandedSkill");
    private MultiPlayerMovement _playerMovement;
    private Weapon _weapon = null;
    public float coolTime = 5.0f; // 쿨타임 (초)
    private bool _isCoolTime = false;
    private Image _coolTimeImage;

    private void Start()
    {
        _coolTimeImage = MultiScene.Instance.skillImages[3];
        _playerMovement = GetComponent<MultiPlayerMovement>();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !_isCoolTime)
        {
            if (_weapon == null) return;

            _playerMovement.navAgent.isStopped = true;
            
            StartCoroutine(CoolDown());
            switch (_weapon.weaponType)
            {
                case WeaponType.Bow:
                    _playerMovement.SetAnimationTrigger(BowSkill);
                    MultiScene.Instance.BroadCastingAnimation(BowSkill, true);
                    break;
                case WeaponType.OneHanded:
                    _playerMovement.SetAnimationTrigger(OneHandedSkill);
                    MultiScene.Instance.BroadCastingAnimation(OneHandedSkill, true);
                    break;
                case WeaponType.TwoHanded:
                    _playerMovement.SetAnimationTrigger(TwoHandedSkill);
                    MultiScene.Instance.BroadCastingAnimation(TwoHandedSkill, true);
                    break;
            }
        }
    }

    public void OnAnimationEnd()
    {
        _playerMovement.navAgent.isStopped = false;
    }

    private IEnumerator CoolDown()
    {
        _isCoolTime = true;
        float timePassed = 0;

        while (timePassed < coolTime)
        {
            timePassed += Time.deltaTime;
            _coolTimeImage.fillAmount = 1 - (timePassed / coolTime);
            yield return null;
        }

        _coolTimeImage.fillAmount = 0;
        _isCoolTime = false;
    }

    public void ChangeWeapon(int index)
    {
        _weapon = MultiScene.Instance.weaponsList[index].GetComponent<Weapon>();

        switch (_weapon.weaponType)
        {
            case WeaponType.Bow:
                MultiScene.Instance.skillImages[0].enabled = true;
                MultiScene.Instance.skillImages[1].enabled = false;
                MultiScene.Instance.skillImages[2].enabled = false;
                break;
            case WeaponType.OneHanded:
                MultiScene.Instance.skillImages[0].enabled = false;
                MultiScene.Instance.skillImages[1].enabled = true;
                MultiScene.Instance.skillImages[2].enabled = false;
                break;
            case WeaponType.TwoHanded:
                MultiScene.Instance.skillImages[0].enabled = false;
                MultiScene.Instance.skillImages[1].enabled = false;
                MultiScene.Instance.skillImages[2].enabled = true;
                break;
            default:
                break;
        }
    }

    
}