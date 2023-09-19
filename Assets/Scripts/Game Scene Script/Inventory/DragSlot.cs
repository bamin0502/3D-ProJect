using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragSlot : MonoBehaviour
{
    //자기 자신을 인스턴스화함 
    public static DragSlot inst;
    public Slot dragSlot;

    [SerializeField]
    private Image imageItem;

    // Start is called before the first frame update
    void Start()
    {
        inst = this;
    }
    public void DragSetImage(Image _itemImage)
    {
        imageItem.sprite = _itemImage.sprite;
        SetColor(1);
    }
    public void SetColor(float _alpha)
    {
        Color color = imageItem.color;
        color.a = _alpha;
        imageItem.color = color;
    }
}
