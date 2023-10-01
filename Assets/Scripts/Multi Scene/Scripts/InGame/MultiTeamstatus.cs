using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class MultiTeamstatus : MonoBehaviour
{
    public string playerName="";
    [SerializeField]
    private TextMeshProUGUI nameText;
    [SerializeField]
    private GameObject statusbar;
    [SerializeField]
    private GridLayoutGroup gridLayoutGroup;
    public Image playerHpImage;
    void Start()
    {
        
    }

    void Awake()
    {
        nameText = GetComponentInChildren<TextMeshProUGUI>();
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
    }

    void Update()
    {
        nameText.text = playerName;
        //playerHpImage.fillAmount = MultiScene.Instance.currentHp / MultiScene.Instance.maxHp;
  
    }
}
