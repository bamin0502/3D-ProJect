using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MagneticField : MonoBehaviour
    {
        //자기장 크기 설정
        public float fieldRange = 200.0f;
        //자기장 세기 설정
        public int fieldIntensity = 1;
        public float damage = 5f;
        public ParticleSystem fieldParticleSystem;
        private SphereCollider fieldCollider;
        private float[] damageMultiplier = { 5f, 10f, 20f, 40f };
        private Color fieldColor;
        private const string PLAYER_TAG = "Player";
        public TMP_Text MagneticNoticeText;
        private float Timer = 120f;
        
        void Start()
        {
            fieldParticleSystem.Stop();
            var shape = fieldParticleSystem.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = fieldRange;
            fieldColor = fieldParticleSystem.main.startColor.color;
            fieldCollider = gameObject.AddComponent<SphereCollider>();
            fieldCollider.radius = fieldRange;
            fieldCollider.isTrigger = true;
            SetActive(active: true);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(PLAYER_TAG))
            {
                PlayerMovement player = other.GetComponent<PlayerMovement>();
                if (player != null)
                {
                    //player.TakeDamage(damage * damageMultiplier[fieldIntensity - 1] * Time.deltaTime);
                }
            }
        }

        public void SetIntensity(int intensity)
        {
            fieldIntensity = intensity;
            float alpha = intensity * 0.25f;
            fieldColor.a = alpha;
            var main = fieldParticleSystem.main;
            main.startColor = fieldColor;
        }

        public void SetActive(bool active)
        {
            if (Timer > 0f)
            {
                MagneticNoticeText.text = "자기장이 생성되었습니다! 안전구역을 확인하세요!";
                fieldParticleSystem.Play();
                StartCoroutine(ScaleDownField());
            }
            else
            {
                fieldParticleSystem.Stop();
            }
        }

        IEnumerator ScaleDownField()
        {
            float duration = 2.0f;
            float scale = fieldIntensity * 0.5f;
            float startTime = Time.time;

            while (Time.time < startTime + duration)
            {
                float t = (Time.time - startTime) / duration;
                fieldParticleSystem.transform.localScale = Vector3.one * Mathf.Lerp(scale, 0f, t);
                yield return null;
            }

            fieldParticleSystem.Stop();
        }

        public void NoticeMagneticFieldText()
        {
            Timer -= Time.deltaTime;
            if (Timer > 0f)
            {
                MagneticNoticeText.text = "자기장 생성까지 " + Mathf.CeilToInt(Timer).ToString() + "초만큼 남았습니다.";
            }
            else
            {
                MagneticNoticeText.text = "자기장이 생성되었습니다! 안전구역을 확인하세요!";
                SetActive(active: true);
            }
        }

    void Update()
        {
            NoticeMagneticFieldText();
        }
    }