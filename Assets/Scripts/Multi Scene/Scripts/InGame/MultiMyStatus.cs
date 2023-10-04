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
    private Camera _cam;
    public void CreateMyStatus(string myPlayerName, Vector3 playerPosition)
    {
        // 각 캐릭터마다 자신만의 UI 요소를 생성하고 위치를 조정
        
        GameObject nameStatus = Instantiate(mynameStatusPrefab, playerPosition + new Vector3(0, 1, 0), Quaternion.identity);
        MultiMyStatus teamStatus = nameStatus.GetComponent<MultiMyStatus>();
        teamStatus.myplayerName = myplayerName;
        teamStatus.mynameText = nameStatus.GetComponentInChildren<TextMeshProUGUI>();
        teamStatus.mynameText.text = myplayerName;

        // 상태창의 회전을 고정,캔버스도 회전을 막아야 함 
        teamStatus.transform.rotation = new Quaternion(0, 180, 0, 0);
        mystatus.transform.rotation = new Quaternion(0, 0, 0, 0);
        
        // mystatus Canvas의 자식으로 추가
        nameStatus.transform.SetParent(mystatus.transform);
        nameStatus.transform.rotation = new Quaternion(0, 0, 0, 0);
    }

    void Start()
    {
        _cam = Camera.main;
    }

    void Awake()
    {
        mystatus = GameObject.FindGameObjectWithTag("MyStatus").GetComponent<Canvas>();
        mynameStatusPrefab = Resources.Load<GameObject>("Mystatus");
        
    }
}
