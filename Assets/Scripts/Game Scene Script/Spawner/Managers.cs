using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers Instance;
    public static Managers GetInstance() { return Instance; }//유일한 매니저를 가지고 옴
    // Start is called before the first frame update
    void Start()
    {
        GameObject obj = GameObject.Find("@Managers");
        Instance = obj.GetComponent<Managers>();//instance에다가 스크립트를 할당한다.
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
