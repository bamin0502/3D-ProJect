using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UImanager : MonoBehaviour
{
    public static UImanager inst;

    public TMP_Text PlayerLevel;
    public TMP_Text PlayerExp;
    
    private void Awake()
    {
        // 이미 인스턴스가 존재하면 중복 생성을 방지하기 위해 파괴
        if (inst != null && inst != this)
        {
            Destroy(this.gameObject);
        }
        inst = this;
        
    }
    public void Start()
    {

    }
    private void Update()
    {
        
    }
    public void UpdatePlayer()
    {
        
    }
}
