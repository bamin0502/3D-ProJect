using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotToolTip : MonoBehaviour
{
    [SerializeField]
    private GameObject go_Base;
    [SerializeField]
    private TMP_Text txt_ItemName;
    [SerializeField]
    private TMP_Text txt_ItemDesc;
    [SerializeField]
    private TMP_Text txt_ItemHowtoUsed;
    
    public void ShowToolTip(Item _item, Vector3 _pos)
    {
        go_Base.SetActive(true);
        _pos += new Vector3(go_Base.GetComponent<RectTransform>().rect.width * 0.5f,
                            -go_Base.GetComponent<RectTransform>().rect.height * 0.5f,
                            0);
        go_Base.transform.position = _pos;

        txt_ItemName.text = _item.itemName;
        txt_ItemDesc.text = _item.itemDesc;

        txt_ItemHowtoUsed.text = _item.itemType == Item.ItemType.Used ? "우 클릭시 해당 아이템 사용" : "";
    }
    public void HideToolTip()
    {
        go_Base.SetActive(false);
    }
}
