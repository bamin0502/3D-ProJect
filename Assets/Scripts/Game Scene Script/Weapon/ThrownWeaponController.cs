using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownWeaponController : MonoBehaviour
{
    //임성훈
    public Camera _cam;  //카메라
    public GameObject grenadePrefab;   // 수류탄 프리팹(나중에 인벤토리에서 가져오는걸로 수정)
    public float maxThrowRange = 10f;  // 최대 던지기 범위
    public LayerMask groundLayer;  // 땅 레이어
    public GameObject throwRangeIndicator;  // 던지기 범위 표시
    public GameObject damageRangeIndicator;  // 데미지 범위 표시
    public bool isGrenadeMode; //던지기 모드
    public float grenadeFlightTime = 2.0f; // 수류탄 날라가는 시간
    public float spinSpeed = 1.0f; // 수류탄 회전속도

  
    void Update()
    {
        if (isGrenadeMode)
        {
            UpdateIndicators();

            if (Input.GetMouseButtonDown(0))
            {
                ThrowGrenade();
                isGrenadeMode = false;
                throwRangeIndicator.SetActive(false);
                damageRangeIndicator.SetActive(false);

            }
        }
        if (Time.timeScale == 0)
        {
            return;
        }
    }

    void ThrowGrenade()
    {
        Vector3 mousePosition = GetMousePositionOnGround();
        float distanceToMouse = Vector3.Distance(mousePosition, transform.position);
        
        if (distanceToMouse > maxThrowRange)
        {
            Vector3 directionToMouse = (mousePosition - transform.position).normalized;
            mousePosition = transform.position + directionToMouse * maxThrowRange;
            distanceToMouse = maxThrowRange;
        }

        GameObject grenade = Instantiate(grenadePrefab, transform.position, Quaternion.identity);
        if (grenade.TryGetComponent(out ThrownWeapon thrownWeapon)) thrownWeapon.canExplode = true;
        
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
    

    void UpdateIndicators()
    {
        Vector3 mousePosition = GetMousePositionOnGround();
        float distanceToMouse = Vector3.Distance(mousePosition, transform.position);
        
        if (distanceToMouse > maxThrowRange)
        {
            Vector3 directionToMouse = (mousePosition - transform.position).normalized;
            mousePosition = transform.position + directionToMouse * maxThrowRange;
        }
        
        throwRangeIndicator.transform.position =
            new Vector3(transform.position.x, 0.01f, transform.position.z);
        throwRangeIndicator.transform.localScale =
            new Vector3(maxThrowRange * 2, 0.001f,
                maxThrowRange * 2);
        
        float damageRadius = grenadePrefab.GetComponent<ThrownWeapon>().explosionRadius;
        
        damageRangeIndicator.transform.position =
            new Vector3(mousePosition.x, 0.02f, mousePosition.z);
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
