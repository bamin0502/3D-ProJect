using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//������ ���ȣ
namespace mino 
{
    public enum PlayerState
    {
        Idle,   //0 ������ �ִ� ���� �����Ұ�
        Walk,   //1 ���콺�� Ŭ���ؼ� �̵��Ҷ� ������ �ִϸ��̼�
        LongAttack, //2 ���Ÿ� ���� ���ݸ�� �ִϸ��̼�
        GunAttackIdle, //3 ����: �� ���� �ִϸ��̼�
        BowAttackIdle,  //4 ���� : Ȱ ���� �ִϸ��̼�
        ShortAttack,   //5  ���� ���� �ִϸ��̼� ���� 
        HabberAttackIdle,   //6 ���� : �ظ� ���� �ִϸ��̼�
        Dead, //7 �׾�����
        SpaceMove, //8 �����̽��ٸ� ������ �� ������ �� �ִϸ��̼�
        ItemUse, //9 �������� ������϶� �����ų �ִϸ��̼� 
    }

    public class AniSetting : MonoBehaviour
    {
        Animator ani;
        // Start is called before the first frame update
        void Start()
        {
            ani = GetComponent<Animator>();
        }
        //enum ���� ���� �ൿ�ϰ� �Ұ���
        public void ChangeAnimation(PlayerState aniNumber)
        {
            //���ϸ��̼��� ���� aniPlayer���� ��ȣ�� ���� �������� �Ұ���!
            ani.SetInteger("aniPlayer", (int)aniNumber);
        }
    }
}



