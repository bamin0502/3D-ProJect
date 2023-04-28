using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 제작자 :방민호
namespace mino {
    public class PlayerMovement : MonoBehaviour
    {
        //플레이어의 애니메이터를 관리
        [Header("Player Animator Setting")]
        [SerializeField]
        //플레이어 애니메이션을 관리
        private Animator AniSetting;
        //플레이어의 기본상태값을 IDLE로 지정함
        //public PlayerState currentState = PlayerState.Idle;

        //플레이어의 움직임을 관리
        [Header ("Player Move")]
        [SerializeField]
        //이동속도 지정 초당 5M의 속도로 이동시킬거임
        private float moveSpeed = 5f;
        //1초에 방향을 회전시킬 기준을 정해준다.
        private float rotAnglePerSecond = 360;
        //마우스 클릭하는 지점을 받아올곳
        private Vector3 curTargetPos;

        //플레이어의 마우스클릭을 관리
        [Header("Monster Click!")]
        [SerializeField]
        //몬스터 관련 오브젝트 생성
        private GameObject ClickEmeny;

        //플레이어에게 보일 UI 이벤트들을 지정해줌
        [Header("UI EVENT")]
        [SerializeField]
        //적에게 준 데미지를 표시해줄 텍스트 지정
        public TMP_Text HitDamage;
        //적에게 받은 데미지를 표시해줄 텍스트 지정
        public TMP_Text VoidDamage;
        //적의 체력을 표시해줄 슬라이더 지정
        public Slider EnemyHPBar;
        //나의 체력을 보이게 해줄 슬라이더를 지정
        public Slider MyHPBar;


        // Start is called before the first frame update
        void Start()
        {
            
            //적의 슬라이더를 초기화 함
            EnemyHPBar=GetComponent<Slider>();
            //나의 슬라이더를 초기화 함
            MyHPBar=GetComponent<Slider>();
            //처음에는 적의 체력바를 안보이도록 한다.
            EnemyHPBar.enabled=true;

        }

        // Update is called once per frame
        void Update()
        {

        }
        //캐릭터를 이동시킬 변수 지정
        public void MovePoint(Vector3 targetpos)
        {
            
        }
        public void Move(Vector3 targetpos)
        {

        }

        
        public void AttackCalculate() 
        {
            
        }
    }

}
