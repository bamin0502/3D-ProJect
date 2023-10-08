using UnityEngine;

public class MultiPlayerSkill : MonoBehaviour
{
    private Weapon _weapon = null;
    //플레이어 스킬 관련
    
    //플레이어 무기 관련
    //플레이어가 스킬 키를 눌렀을때 액션

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

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if(_weapon == null) return;

            switch (_weapon.weaponType)
            {
                case WeaponType.Bow:
                    break;
                case WeaponType.OneHanded:
                    break;
                case WeaponType.TwoHanded:
                    break;
            }
        }
    }
}