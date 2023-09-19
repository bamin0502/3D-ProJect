using System.Collections;
using UnityEngine;

public class BossRock : MonoBehaviour
{
    Rigidbody rigid;
    float angularPower = 2;
    float scaleValue = 0.1f;
    bool isShoot;
    public int damage;
    public Transform target;
    bool isRock;
    private void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        rigid = GetComponent<Rigidbody>();
        StartCoroutine(GainPowerTimer());
        StartCoroutine(GainPower());
    }

    IEnumerator GainPowerTimer()
    {
        yield return new WaitForSeconds(2.2f);
        isShoot = true;
    }

    IEnumerator GainPower()
    {
        while (!isShoot)
        {
            angularPower += 0.02f;
            scaleValue += 0.005f;
            transform.localScale = Vector3.one * scaleValue;
            rigid.AddTorque(transform.right * angularPower, ForceMode.Acceleration);
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = target.position - transform.position;
        float distanceToTarget = direction.magnitude;
        if (distanceToTarget < 0.5f)  // 예시로 거리 0.5f 이내로 충돌을 감지하도록 설정
        {
            HitTarget();
        }
    }
    private void HitTarget()
    {
        Destroy(gameObject);

    }
}