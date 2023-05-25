using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlotController : MonoBehaviour
{
    [SerializeField] private Slot[] quickSlots;
    [SerializeField] private Transform tf_parent;
    [SerializeField] private Image[] img_coolTime;

    private int selectedSlot;
    [SerializeField] private GameObject go_SelectedImage;

    [SerializeField]
    private float coolTime;
    [SerializeField]
    private float currentCoolTime;
    [SerializeField]
    private bool isCoolTime;
    [SerializeField]
    private DataManager theItemEffectDatabase;
    // Start is called before the first frame update
    void Start()
    {
        TryInputNumber();
        CoolTimeCalc();
    }
    private void CoolTimeReset()
    {
        currentCoolTime = coolTime;
        isCoolTime = true;
    }
    private void CoolTimeCalc()
    {
        if(isCoolTime)
        {
            currentCoolTime -= Time.deltaTime;

            for(int i=0; i<img_coolTime.Length; i++)
            {
                img_coolTime[i].fillAmount = currentCoolTime / coolTime;

                if (currentCoolTime <= 0)
                {
                    isCoolTime=false;
                }
            }
        }
    }
    private void TryInputNumber()
    {
        if (!isCoolTime)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ChangeSlot(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ChangeSlot(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                ChangeSlot(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                ChangeSlot(3);
            }
        }
    }
    private void ChangeSlot(int _num)
    {
        SelectedSlot(_num);
        Execute();
    }
    private void SelectedSlot(int _num)
    {
        selectedSlot = _num;


    }
    private void Execute()
    {
        CoolTimeReset();

        if (quickSlots[selectedSlot].item != null)
        {
            if (quickSlots[selectedSlot].item.itemType==Item.ItemType.Used 
                || quickSlots[selectedSlot].item.itemType == Item.ItemType.buff 
                || quickSlots[selectedSlot].item.itemType == Item.ItemType.Throw)
            {
                EatItem();
            }
        }
    }
    public void EatItem()
    {
        CoolTimeReset();
        theItemEffectDatabase.UseItem(quickSlots[selectedSlot].item);
        quickSlots[selectedSlot].SetSlotCount(-1);

    }
    public bool GetIsCoolTime()
    {
        return isCoolTime;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
