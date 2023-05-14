using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjManager : MonoBehaviour
{
    public static ObjManager instance;
    public Sprite[] spr;
    public Inventory Iv;
    
   
    public static ObjManager Call()
    {
        if (instance == null)
        {
            instance = new ObjManager();
            
        }
        return instance;
        
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
}
