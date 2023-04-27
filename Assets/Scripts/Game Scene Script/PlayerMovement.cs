using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// ������ :���ȣ
namespace mino {

    public class PlayerMovement : MonoBehaviour
    {
        //�÷��̾��� �ִϸ����͸� ����
        [Header("Player Animator Setting")]
        [SerializeField]
        //�÷��̾� �ִϸ��̼��� ����
        private Animator AniSetting;
        //�÷��̾��� �⺻���°��� IDLE�� ������
        //public PlayerState currentState = PlayerState.Idle;

        //�÷��̾��� �������� ����
        [Header ("Player Move")]
        [SerializeField]
        //�̵��ӵ� ���� �ʴ� 5M�� �ӵ��� �̵���ų����
        private float moveSpeed = 5f;
        //1�ʿ� ������ ȸ����ų ������ �����ش�.
        private float rotAnglePerSecond = 360;
        //���콺 Ŭ���ϴ� ������ �޾ƿð�
        private Vector3 curTargetPos;

        //�÷��̾��� ���콺Ŭ���� ����
        [Header("Monster Click!")]
        [SerializeField]
        //���� ���� ������Ʈ ����
        private GameObject ClickEmeny;

        //�÷��̾�� ���� UI �̺�Ʈ���� ��������
        [Header("UI EVENT")]
        [SerializeField]
        //������ �� �������� ǥ������ �ؽ�Ʈ ����
        public TMP_Text HitDamage;
        //������ ���� �������� ǥ������ �ؽ�Ʈ ����
        public TMP_Text VoidDamage;
        //���� ü���� ǥ������ �����̴� ����
        public Slider EnemyHPBar;
        //���� ü���� ���̰� ���� �����̴��� ����
        public Slider MyHPBar;



        // Start is called before the first frame update
        void Start()
        {
            
            //���� �����̴��� �ʱ�ȭ ��
            EnemyHPBar=GetComponent<Slider>();
            //���� �����̴��� �ʱ�ȭ ��
            MyHPBar=GetComponent<Slider>();
            //ó������ ���� ü�¹ٸ� �Ⱥ��̵��� �Ѵ�.
            EnemyHPBar.enabled=true;

        }

        // Update is called once per frame
        void Update()
        {

        }
        //ĳ���͸� �̵���ų ���� ����
        public void MovePoint(Vector3 targetpos)
        {
            
        }
        public void Move(Vector3 targetpos)
        {

        }
        
        public void AttackCalculate() {
            
        }
    }

}
