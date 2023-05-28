using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    public Boss boss; // 보스 클래스 참조 변수 추가
    public Image bossHealthBar;

    [SerializeField]
    private Camera _cam;

    public void UpdateHealth()
    {
        bossHealthBar.fillAmount = (float)boss.curHealth / boss.maxHealth; // 보스의 현재 체력 값 사용
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 lookAtPosition = _cam.transform.position;
        lookAtPosition.x = transform.position.x;
        transform.LookAt(lookAtPosition);
    }
}