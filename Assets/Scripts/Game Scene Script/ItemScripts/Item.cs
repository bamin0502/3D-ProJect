using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum TYPE { HP, MP }

    public TYPE type;           // 아이템의 타입.
    public Sprite DefaultImg;   // 기본 이미지.
    public int MaxCount;        // 겹칠수 있는 최대 숫자.  

    public string Name;         // 아이템의 이름.
    
    public void Init(string Name, int MaxCount)
    {
        // 이름에 따라 타입을 결정.
        switch (Name)
        {
            case "HP": type = TYPE.HP; break;
            case "MP": type = TYPE.MP; break;
        }

        this.Name = Name;           // 이름을 초기화.
        this.MaxCount = MaxCount;   // 겹칠수 있는 한계 개수를 초기화.


        Sprite[] spr = ObjManager.Call().spr;       // 이미지 배열을 가져온다.
        int Count = ObjManager.Call().spr.Length;   // 이미지 배열의 총 크기.
                                                    // 이미지의 이름과 아이템의 이름을 비교.
        for (int i = 0; i < Count; i++)
        {
            // 두 이름이 같으면 그 이미지를 DefaultImg에 셋팅.
            if (spr[i].name == this.Name)
            {
                DefaultImg = spr[i];
                break;
            }
        }
    }

    // 인벤토리에 접근하기 위한 변수.
    private Inventory Iv;

    void Awake()
    {
        // 태그명이 "Inventory"인 객체의 GameObject를 반환한다.
        // 반환된 객체가 가지고 있는 스크립트를 GetComponent를 통해 가져온다.
        Iv = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();

    }

    void AddItem()
    {
        // 아이템 획득에 실패할 경우.
        if (!Iv.AddItem(this))
            Debug.Log("아이템이 가득 찼습니다.");
        else // 아이템 획득에 성공할 경우.
            gameObject.SetActive(false); // 아이템을 비활성화 시켜준다.
    }
    // 충돌체크
    void OnTriggerEnter(Collider _col)
    {
        // 플레이어와 충돌하면.
        if (_col.gameObject.layer == 6)
            AddItem();
    }
}