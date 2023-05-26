using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//제작자: 방민호

    public class ItemSpawner : MonoBehaviour
    {
        //생성할 아이템을 리스트배열 형식으로 저장한다.
        public GameObject[] Items;
        //생성할 아이템의 위치를 리스트배열 형식으로 저장한다.
        public Transform[] ItemPosition;
        //몬스터의 위치값을 저장할 리스트를 만듬
        public LinkedList<GameObject> MonsterPosition;
        // Start is called before the first frame update
        void Start()
        {
            ItemSpawn();
        }
        //아이템을 생성 처리
        private void ItemSpawn()
        {
            //몬스터의 위치값을 가져올거임
            Vector3 spawnPosition = GetComponent<GameObject>().transform.position;
            //몬스터,혹은 적 플레이어가 죽었을때 그 몬스터 주위로 아이템을 뿌릴거임(일단은 비활성화)
            //GameObject selectItem = Items[Dead.Range(0, Items.Length)];
            //GameObject Item = Instantiate(selectItem,spawnPosition,Quaternion.identity);
        }
    }



