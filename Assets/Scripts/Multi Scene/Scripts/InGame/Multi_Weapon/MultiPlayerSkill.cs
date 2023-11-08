using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MultiPlayerSkill : MonoBehaviour
{
    //플레이어 스킬 관련
    private static readonly int OneHandedSkill = Animator.StringToHash("OneHandedSkill");
    private static readonly int TwoHandedSkill = Animator.StringToHash("TwoHandedSkill");
    private MultiPlayerMovement _playerMovement;
    private MultiWeaponController _currentWeapon;
    private ThrownWeaponController _thrownWeaponController;
    private Weapon _weapon = null;
    public float coolTime = 15.0f; // 쿨타임 (초)
    private bool _isCoolTime = false;
    private Image _coolTimeImage;

    public ParticleSystem[] effects; //0검, 1망치
    
    private void Start()
    {
        _coolTimeImage = MultiScene.Instance.skillImages[3];
        MultiScene.Instance.skillText.enabled = false;
        _playerMovement = GetComponent<MultiPlayerMovement>();
        _currentWeapon = GetComponent<MultiWeaponController>();
        _thrownWeaponController = MultiScene.Instance.currentThrownWeaponController;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !_isCoolTime)
        {
            if (_weapon == null) return;
            
            Skill(NetGameManager.instance.m_userHandle.m_szUserID);
            MultiScene.Instance.BroadCastingPlayerSkill();
        }
    }

        

    public void Skill(string userID)
    {
        if (_currentWeapon.equippedWeapon == null || _isCoolTime) return;
        
        _playerMovement.navAgent.isStopped = true;

        StartCoroutine(CoolDown(userID));
        
        switch (_currentWeapon.equippedWeapon.weaponType)
        {
            case WeaponType.Bow:
                if (!MultiScene.Instance.currentUser.Equals(userID)) return;
                
                if (!_thrownWeaponController.isGrenadeMode)
                {
                    _thrownWeaponController.isGrenadeMode = true;
                    _thrownWeaponController.throwMode = 1;
                    _thrownWeaponController.throwRangeIndicator.SetActive(true);
                    _thrownWeaponController.damageRangeIndicator.SetActive(true);
                }
                break;
            case WeaponType.OneHanded:
                _playerMovement.SetAnimationTrigger(OneHandedSkill);
                break;
            case WeaponType.TwoHanded:
                _playerMovement.SetAnimationTrigger(TwoHandedSkill);
                break;
        }
    }

    public void SkillStart()
    {
        switch (_currentWeapon.equippedWeapon.weaponType)
        {
            case WeaponType.Bow:
                break;
            case WeaponType.OneHanded:
                effects[0].Play();
                break;
            case WeaponType.TwoHanded:
                effects[1].Play();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void OnAnimationEnd()
    {
        _playerMovement.navAgent.isStopped = false;
    }

    private IEnumerator CoolDown(string userID)
    {
        _isCoolTime = true;
        float timePassed = 0;

        while (timePassed < coolTime)
        {
            timePassed += Time.deltaTime;
            if (userID.Equals(NetGameManager.instance.m_userHandle.m_szUserID))
            {
                _coolTimeImage.fillAmount = 1 - (timePassed / coolTime);
            }
            
            yield return null;
        }

        if (userID.Equals(NetGameManager.instance.m_userHandle.m_szUserID))
        {
            _coolTimeImage.fillAmount = 0;
        }
        
        _isCoolTime = false;
    }

    public void ChangeWeapon(int index)
    {
        _weapon = MultiScene.Instance.weaponsList[index].GetComponent<Weapon>();
        MultiScene.Instance.skillText.enabled = true;
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