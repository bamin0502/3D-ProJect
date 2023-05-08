using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    //임성훈 
    public string weaponName; //무기명
    public int damage; //데미지
    public bool isEquipped = false; //장착 여부
    public float attackInterval;

    private Texture2D hand; //줍기 마우스
    private Texture2D original; //기본 마우스
    void Start()
    {
        hand = Resources.Load<Texture2D>("Cursor_Hand");
        original = Resources.Load<Texture2D>("Cursor_Basic");
    }

    void OnMouseEnter()
    {
        Cursor.SetCursor(hand, new Vector2(hand.width / 3, 0), CursorMode.Auto);
    }

    void OnMouseExit()
    {
        Cursor.SetCursor(original, new Vector2(hand.width / 3, 0), CursorMode.Auto);
    }


    public abstract void Attack(Transform target);
    
    
}
