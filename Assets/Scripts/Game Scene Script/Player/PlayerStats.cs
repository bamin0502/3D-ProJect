using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[System.Serializable]
public class PlayerStats : MonoBehaviour
{
    public string playerName; //�÷��̾� �̸�
    public int playerLevel; //�÷��̾� ����
    public float health; //ü��
    public float attack; //���ݷ�
    public float speed; //�̵��ӵ�
    public float PlayerExp; //����ġ
    void Start()
    {
        
    }
}
