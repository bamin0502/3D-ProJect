using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownWeaponController : MonoBehaviour
{
    //임성훈
    public GameObject grenadePrefab;
    public float maxThrowRange = 10f;
    public LayerMask groundLayer;
    public GameObject throwRangeIndicator;
    public GameObject damageRangeIndicator;
    private bool isGrenadeMode;

    void Update()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            isGrenadeMode = true;
            throwRangeIndicator.SetActive(true);
            damageRangeIndicator.SetActive(true);
        }
        else
        {
            isGrenadeMode = false;
            throwRangeIndicator.SetActive(false);
            damageRangeIndicator.SetActive(false);
        }

        if (isGrenadeMode)
        {
            UpdateIndicators();

            if (Input.GetMouseButtonDown(0))
            {
                ThrowGrenade();
            }
        }
    }

    void ThrowGrenade()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
            Camera.main.transform.position.y));
        
        GameObject grenade = Instantiate(grenadePrefab, transform.position, Quaternion.identity);
        
        Vector3 direction = (mousePosition - transform.position).normalized;

        grenade.GetComponent<Rigidbody>().velocity = direction * maxThrowRange;
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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            Debug.Log(hit.point);
            return hit.point;
        }

        return Vector3.zero;
    }
}
