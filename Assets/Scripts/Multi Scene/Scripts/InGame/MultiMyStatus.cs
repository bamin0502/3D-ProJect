using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class MultiMyStatus : MonoBehaviour
{
    
    public string myplayerName = "";
    public TextMeshProUGUI mynameText;
    public Canvas mystatus;
    public GameObject mynameStatusPrefab;
    
    public void CreateMyStatus(string myPlayerName)
    {
        // 각 캐릭터마다 자신만의 UI 요소를 생성하도록 수정
        GameObject nameStatus = Instantiate(mynameStatusPrefab, transform.position + new Vector3(0, 3, 0), Quaternion.identity);
        MultiMyStatus teamStatus = nameStatus.GetComponent<MultiMyStatus>();
        teamStatus.myplayerName = myplayerName;
        teamStatus.mynameText = nameStatus.GetComponentInChildren<TextMeshProUGUI>();
        teamStatus.mynameText.text = myplayerName;
        nameStatus.transform.SetParent(mystatus.transform);
        
        // 상태창의 회전을 고정
        teamStatus.transform.rotation = new Quaternion(0,180,0 ,0);
    }

    

    void Awake()
    {
        mystatus = GameObject.FindGameObjectWithTag("MyStatus").GetComponent<Canvas>();

    }
}
