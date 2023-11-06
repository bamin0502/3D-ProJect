using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
public class HideObject : MonoBehaviour
{
    private HideObject _hideObject;
    private static readonly Dictionary<Collider,HideObject>hideObjectsMap = new Dictionary<Collider, HideObject>();
    
    //숨길 오브젝트 지정
    [SerializeField] public GameObject Renderers;
    public Collider Collider = null;
    void Start()
    {
       InitHideObject(); 
    }

    public static void InitHideObject()
    {
        foreach (var obj in hideObjectsMap.Values.Where(obj => obj != null && obj.Renderers != null))
        {
            obj.SetVisible(true);
            obj._hideObject = null;
        }
        
        hideObjectsMap.Clear();
        
        foreach (var obj in FindObjectsOfType<HideObject>())
        {
            if (obj.Collider != null)
            {
                hideObjectsMap[obj.Collider]=obj;
            }   
        }
    }
   
    public static HideObject GetHideObject(Collider collider)
    {
        return hideObjectsMap.TryGetValue(collider, out var obj) ? GetRoot(obj) : null;
    }

    public static HideObject GetRoot(HideObject obj)
    {
        while (true)
        {
            if (obj._hideObject == null)
                return obj;
            else
            {
                obj = obj._hideObject;
            }
        }
    }

    public void SetVisible(bool visible)
    {
       Renderer rend= Renderers.GetComponent<Renderer>();

       if (rend != null && rend.gameObject.activeInHierarchy && hideObjectsMap.ContainsKey(rend.GetComponent<Collider>()))
       {
           rend.shadowCastingMode = visible ? ShadowCastingMode.On : ShadowCastingMode.ShadowsOnly;
       }
           
    }
}
