using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class ThrownWeaponController : MonoBehaviour
{
    //임성훈
    private static readonly int BowSkillHash = Animator.StringToHash("BowSkill");
    public Camera _cam;  //카메라
    public GameObject grenadePrefab;   // 수류탄 프리팹(나중에 인벤토리에서 가져오는걸로 수정)
    public float maxThrowRange = 9f;  // 최대 던지기 범위
    public LayerMask groundLayer;  // 땅 레이어
    public GameObject throwRangeIndicator;  // 던지기 범위 표시
    public GameObject damageRangeIndicator;  // 데미지 범위 표시
    public bool isGrenadeMode; //던지기 모드
    public int throwMode = 0;
    public float grenadeFlightTime = 2.0f; // 수류탄 날라가는 시간
    public float spinSpeed = 1.0f; // 수류탄 회전속도
    public GameObject bowSkillEffect;
    private MultiPlayerMovement _playerMovement;
    private MultiWeaponController _currentWeapon;
    private readonly Collider[] enemies = new Collider[20];
    public LayerMask enemyLayer;
    private Vector3 _mousePos;

    private void Start()
    {
        _playerMovement = GetComponent<MultiPlayerMovement>();
        _currentWeapon = GetComponent<MultiWeaponController>();
    }

    void Update()
    {
        if (isGrenadeMode)
        {
            UpdateIndicators();

            if (Input.GetMouseButtonDown(0))
            {
                if (throwMode == 0)
                {
                    var position = transform.position;
                    MultiScene.Instance.BroadCastingThrowWeapon(GetMousePositionOnGround(), position, 0);
                    ThrowGrenade(GetMousePositionOnGround(), position);
                }
                else if (throwMode == 1)
                {
                    _mousePos = GetMousePositionOnGround();
                    MultiScene.Instance.BroadCastingThrowWeapon(_mousePos, transform.position, 1);
                    BowSkill();
                }
                
                isGrenadeMode = false;
                throwRangeIndicator.SetActive(false);
                damageRangeIndicator.SetActive(false);
            }
        }
    }

    public void BowSkill()
    {
        _playerMovement.SetAnimationTrigger(BowSkillHash);
    }

    public void SetMousePos(Vector3 pos)
    {
        _mousePos = pos;
    }

    public void SkillStart()
    {
        switch (_currentWeapon.equippedWeapon.weaponType)
        {
            case WeaponType.Bow:
                BowSkill(_mousePos);
                break;
        }
    }

    public void ThrowGrenade(Vector3 mousePosition, Vector3 playerPos)
    {
        float distanceToMouse = Vector3.Distance(mousePosition, playerPos);
        
        if (distanceToMouse > maxThrowRange)
        {
            var position = playerPos;
            Vector3 directionToMouse = (mousePosition - position).normalized;
            mousePosition = position + directionToMouse * maxThrowRange;
            distanceToMouse = maxThrowRange;
        }

        GameObject grenade = Instantiate(grenadePrefab, transform.position, Quaternion.identity);
        SoundManager.instance.PlaySE("Fire_shot");
        
        
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        rb.useGravity = true;

        Vector3 direction = (mousePosition - transform.position).normalized;

        float velocityXZ = distanceToMouse / grenadeFlightTime;
        float velocityY = 0.5f * Mathf.Abs(Physics.gravity.y) * grenadeFlightTime;

        Vector3 velocity = new Vector3(direction.x * velocityXZ, velocityY, direction.z * velocityXZ);
        
        rb.velocity = velocity;

        //수류탄에 랜덤으로 회전 추가
        rb.AddTorque(Random.insideUnitSphere * spinSpeed, ForceMode.VelocityChange);

    }

    public void BowSkill(Vector3 mousePosition)
    {
        float distanceToMouse = Vector3.Distance(mousePosition, transform.position);

        if (distanceToMouse > maxThrowRange)
        {
            var position = transform.position;
            Vector3 directionToMouse = (mousePosition - position).normalized;
            mousePosition = position + directionToMouse * maxThrowRange;
        }

        GameObject arrowSkill = Instantiate(bowSkillEffect, mousePosition, Quaternion.identity);
        ArrowSkillDamage(mousePosition);
        PlayParticleSystems(arrowSkill);
    }

    private void ArrowSkillDamage(Vector3 mousePosition)
    {
        float damageRadius = grenadePrefab.GetComponent<ThrownWeapon>().explosionRadius;
        
        int numColliders = Physics.OverlapSphereNonAlloc(mousePosition, damageRadius, enemies, enemyLayer);

        for (int i = 0; i < numColliders; i++)
        {
            Collider target = enemies[i];
            bool isEnemy = target.TryGetComponent(out EnemyHealth enemy);
            if (isEnemy)
            {
                enemy.TakeDamage(_currentWeapon.equippedWeapon.GetSkillDamage());
            }
        }
    }

    private IEnumerator DeleteParticleCoroutine(GameObject particle, float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(particle);
    }
    
    private void PlayParticleSystems(GameObject arrowSkill)
    {
        Transform[] myChildren = arrowSkill.GetComponentsInChildren<Transform>();
        foreach (Transform child in myChildren)
        {
            child.TryGetComponent(out ParticleSystem arrowParticle);
            if (arrowParticle != null)
            {
                arrowParticle.Play();
                StartCoroutine(DeleteParticleCoroutine(arrowSkill, arrowParticle.main.duration));
                return;
            }
        }
    }

    void UpdateIndicators()
    {
        Vector3 mousePosition = GetMousePositionOnGround();
        float distanceToMouse = Vector3.Distance(mousePosition, transform.position);
        
        if (distanceToMouse > maxThrowRange)
        {
            var position = transform.position;
            Vector3 directionToMouse = (mousePosition - position).normalized;
            mousePosition = position + directionToMouse * maxThrowRange;
        }

        var transform1 = transform;
        var position1 = transform1.position;
        throwRangeIndicator.transform.position =
            new Vector3(position1.x, mousePosition.y + 0.02f, position1.z);
        throwRangeIndicator.transform.localScale =
            new Vector3(maxThrowRange * 2, 0.001f,
                maxThrowRange * 2);
        
        float damageRadius = grenadePrefab.GetComponent<ThrownWeapon>().explosionRadius;
        
        damageRangeIndicator.transform.position =
            new Vector3(mousePosition.x, mousePosition.y + 0.02f, mousePosition.z);
        damageRangeIndicator.transform.localScale =
            new Vector3(damageRadius * 2, 0.001f,
                damageRadius * 2);
    }

    Vector3 GetMousePositionOnGround()
    {
        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            return hit.point;
        }

        return Vector3.zero;
    }
}
