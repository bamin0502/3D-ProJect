using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//������: ���ȣ
namespace mino
{
    public class ItemSpawner : MonoBehaviour
    {
        //������ �������� ����Ʈ�迭 �������� �����Ѵ�.
        public GameObject[] Items;
        //������ �������� ��ġ�� ����Ʈ�迭 �������� �����Ѵ�.
        public Transform[] ItemPosition;
        //������ ��ġ���� ������ ����Ʈ�� ����
        public LinkedList<GameObject> MonsterPosition;
        // Start is called before the first frame update
        void Start()
        {
            ItemSpawn();
        }
        //�������� ���� ó��
        private void ItemSpawn()
        {
            //������ ��ġ���� �����ð���
            Vector3 spawnPosition = GetComponent<GameObject>().transform.position;
            //����,Ȥ�� �� �÷��̾ �׾����� �� ���� ������ �������� �Ѹ�����(�ϴ��� ��Ȱ��ȭ)
            //GameObject selectItem = Items[Dead.Range(0, Items.Length)];
            //GameObject Item = Instantiate(selectItem,spawnPosition,Quaternion.identity);
        }
    }

}

