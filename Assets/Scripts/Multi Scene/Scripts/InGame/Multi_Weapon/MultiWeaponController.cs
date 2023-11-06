using System.Collections;
using UnityEngine;
using UnityEngine.AI;
#if UNITY_EDITOR
using UnityEditor;
#endif


[RequireComponent(typeof(MultiPlayerMovement))]
public class MultiWeaponController : MonoBehaviour
{
    //임성훈
    private const float WeaponPickupRange = 2f;
    private const string WeaponTag = "Weapon";
    private const string Untagged = "Untagged";
    private static readonly int Pickup = Animator.StringToHash("ItemPickup");
    public Weapon equippedWeapon;
    public bool isAttack;
    
    public Transform weaponHolder;
    public Transform weaponHolder2;
    public Transform currentTarget;
    
    private MultiPlayerSkill _playerSkill;
    private MultiPlayerMovement _playerMovement;
    private NavMeshAgent _agent;
    private float _attackTimer;
    private bool _isInRangeToPickup = false;
    private MultiScene _multiScene;
    private Collider[] _itemColliders = new Collider[1];

    private void Start()
    {
        _playerSkill = GetComponent<MultiPlayerSkill>();
        _playerMovement = GetComponent<MultiPlayerMovement>();
        _agent = _playerMovement.navAgent;
        _multiScene = MultiScene.Instance;
    }

    private void Update()
    {
        if (currentTarget != null)
        {
            AttackTarget();
        }

        if (_multiScene.currentUser != transform.gameObject.name) return;

        if (Input.GetKeyDown(KeyCode.G))
        {
            if(_isInRangeToPickup) TryPickupWeapon();
        }
    }

    private void ShowPickupText(bool isShow)
    {
        if (!_multiScene.noticeText.gameObject.activeSelf && isShow)
        {
            _multiScene.noticeText.gameObject.SetActive(true);
            _multiScene.noticeText.text = "무기 줍기" + "<color=yellow>" + "(G)" + "</color>";
        }
        else if(_multiScene.noticeText.gameObject.activeSelf && !isShow)
        {
            _multiScene.noticeText.text = "";
            _multiScene.noticeText.gameObject.SetActive(false);
        }
    }

    public void SetTarget(int enemy, bool isBoss = false)
    {
        if (isBoss)
        {
            var target = _multiScene.bossObject;
            
            if (target == null)
            {
                currentTarget = null;
                return;
            }

            if (target != null)
            {
                if (equippedWeapon == null) return;
                currentTarget = target.transform;
                _agent.stoppingDistance = GetWeaponRange();
                var position = currentTarget.position;
                _agent.SetDestination(position);
                _attackTimer = 0f;

                var range = GetWeaponRange();
                float distance = Vector3.Distance(transform.position, position);
                isAttack = distance <= range;
            }
        }
        
        if (enemy >= 0 && enemy < _multiScene.enemyList.Count)
        {
            var target = _multiScene.enemyList[enemy];

            if (target == null)
            {
                Debug.LogWarning("적이 null입니다. 인덱스: " + enemy);
                currentTarget = null;
                return; 
            }
            if (target != null)
            {
                if(equippedWeapon == null) return;
                currentTarget = target.transform;
                _agent.stoppingDistance = GetWeaponRange();
                var position = currentTarget.position;
                _agent.SetDestination(position);
                _attackTimer = 0f;
        
                var range = GetWeaponRange();
                float distance = Vector3.Distance(transform.position, position);
                isAttack = distance <= range;
            }
            else
            {
                Debug.LogWarning("Enemy 컴포넌트를 찾을 수 없습니다. GameObject 이름: " + target.name);
            }
        }
        else
        {
            Debug.LogWarning("적의 인덱스가 범위를 벗어났습니다. 인덱스: " + enemy);
        }
    }

    private float GetWeaponRange()
    {
        if (equippedWeapon == null) return 0;
        return equippedWeapon is RangedWeapon weapon ? weapon.range + 0.8f : ((MeleeWeapon)equippedWeapon).range;
    }
    public void ClearTarget()
    {
        currentTarget = null;
        isAttack = false;
        _agent.stoppingDistance = 0f;
    }

    public IEnumerator CheckCanPickupWeapon()
    {
        WaitForSeconds wait = new WaitForSeconds(1f);
        
        while (true)
        {
            _itemColliders = new Collider[1];
            int weaponCount = Physics.OverlapSphereNonAlloc(transform.position, WeaponPickupRange, _itemColliders);
            _isInRangeToPickup = weaponCount > 0 && _itemColliders[0].CompareTag("Weapon");
            ShowPickupText(_isInRangeToPickup);
            
            yield return wait;
        }
    }

    private void TryPickupWeapon()
    {
        _playerMovement.SetAnimationTrigger(Pickup);
        _multiScene.BroadCastingAnimation(Pickup, true);

        int index = _multiScene.weaponsList.IndexOf(_itemColliders[0].gameObject);

        PickWeapon(index);
        _playerSkill.ChangeWeapon(index);
        _multiScene.BroadCastingPickWeapon(index);
    }

    public void PickWeapon(int index)
    {
        EquipWeapon(_multiScene.weaponsList[index].GetComponent<Weapon>());
    }
    
    private void AttackTarget()
    {
        if (Vector3.Distance(transform.position, currentTarget.position) <= _agent.stoppingDistance)
        {
            Quaternion targetRotation = Quaternion.LookRotation(currentTarget.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }
    
    private void EquipWeapon(Weapon newWeapon)
    {
        if (equippedWeapon != null) DropEquippedWeapon();
        
        if(newWeapon != null)
        {
            newWeapon.tag = Untagged;
            newWeapon.isEquipped = true;
            
            if(newWeapon.TryGetComponent(out Collider collider)) collider.enabled = false;

            Transform weaponTransform = newWeapon.transform;
            weaponTransform.parent = newWeapon.weaponType != WeaponType.Bow ? weaponHolder : weaponHolder2;
            weaponTransform.localPosition = newWeapon.PickPosition;
            weaponTransform.localRotation = Quaternion.Euler(newWeapon.PickRotation);
            equippedWeapon = newWeapon;
            
            if (equippedWeapon.iconCanvas != null)
            {
                OnOffCanvas(equippedWeapon.iconCanvas, false);
            }
        }
    }

    private void DropEquippedWeapon()
    {
        if(equippedWeapon == null) return;
        if (equippedWeapon.iconCanvas != null) OnOffCanvas(equippedWeapon.iconCanvas);

        equippedWeapon.tag = WeaponTag;
        equippedWeapon.isEquipped = false;
        equippedWeapon.transform.SetParent(null);
        
        if (equippedWeapon.TryGetComponent(out Collider collider)) collider.enabled = true;

        Transform transform2;
        (transform2 = equippedWeapon.transform).rotation = Quaternion.Euler(90f, 0f, 0f);
        var transform1 = transform;
        transform2.position = transform1.position + transform1.forward;
        
        equippedWeapon = null;
    }
    
    private void OnOffCanvas(Canvas canvas, bool isOn = true)
    {
        canvas.enabled = isOn;
    }
}
        
    
