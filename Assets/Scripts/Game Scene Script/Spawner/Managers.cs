using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers Instance;
    public static Managers GetInstance() { return Instance; }//������ �Ŵ����� ������ ��
    // Start is called before the first frame update
    void Start()
    {
        GameObject obj = GameObject.Find("@Managers");
        Instance = obj.GetComponent<Managers>();//instance���ٰ� ��ũ��Ʈ�� �Ҵ��Ѵ�.
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
