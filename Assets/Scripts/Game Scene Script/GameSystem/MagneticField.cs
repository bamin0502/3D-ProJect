using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MagneticField : MonoBehaviour
{
    //자기장의 범위
    public float fieldRange = 200.0f;
    //자기장의 세기, (1~4)단계로 구분예정
    public int fieldIntensity = 1;
    //초기 자기장 데미지 지정
    public float damage = 5f;
    //자기장 파티클 시스템
    public ParticleSystem fieldParticleSystem;
    //데미지 처리할 때 사용할 콜라이더
    private SphereCollider fieldCollider;
    //데미지 지정
    private float[] damageMultiplier = { 5f, 10f, 20f, 40f };
    //자기장 색상 지정
    private Color fieldColor;
    //플레이어를 태그함
    private const string PLAYER_TAG = "Player";

    public TMP_Text MagneticNoticeText ;

    private float Timer=120f;
    // Start is called before the first frame update
    void Start()
    {
        //처음에는 파티클 시스템 설정함
        fieldParticleSystem.Stop();
        var shape = fieldParticleSystem.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = fieldRange;
        fieldColor = fieldParticleSystem.main.startColor.color;

        //콜라이더도 마찬가지로 설정해줘야한다.
        fieldCollider = gameObject.AddComponent<SphereCollider>();
        fieldCollider.radius = fieldRange;
        //자기장과 플레이어가 충돌되지않도록 켜주어야한다. false일시 충돌됨
        fieldCollider.isTrigger = true;


    }
    //자기장 콜라이더 밖에 있는 것을 검사할거임 그래야 자기장 시스템에 맞음
    private void OnTriggerStay(Collider other)
    {
        //밖에 검색을 해서 플레이어가 있을시 
        if (other.CompareTag(PLAYER_TAG))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.TakeDamage(damage * damageMultiplier[fieldIntensity - 1] * Time.deltaTime);

            }
        }
    }
    public void SetIntensity(int intensity)
    {
        fieldIntensity = intensity;

        //파티클 시스템 색상 변경시킴
        float alpha = intensity * 0.25f;
        fieldColor.a = alpha;
        var main = fieldParticleSystem.main;
        main.startColor = fieldColor;

    }
    public void SetActive(bool active)
    {
        //타이머가 120초가(2분)이 넘기전까지는 생성을 시키지않을거임
        if (Timer > 0f)
        {
            Timer -= Time.deltaTime;
            MagneticNoticeText.text = "자기장 생성까지 " + Mathf.CeilToInt(Timer).ToString() + "초 남았습니다.";
            fieldParticleSystem.Play();
        }
        else
        {
            fieldParticleSystem.Stop();
        }
    }
    public void NoticeMagneticFieldText()
    {
        //처음에 시작할때 타이머 작동시킬거임
        Timer = Timer-Time.deltaTime;
        //그 타이머의 값을 가져올거임
        MagneticNoticeText.text = "자기장 생성까지" +Timer.ToString()+"만큼 남았습니다.";

    }
    // Update is called once per frame
    void Update()
    {
        NoticeMagneticFieldText();       
    }
}
