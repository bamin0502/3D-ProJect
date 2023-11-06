using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectHideCamera : MonoBehaviour
{
    public Transform target = null;
    
    //이 감지거리 안에 플레이어가 있으면 해당 구조물을 안보이게 할거임 
    [SerializeField] private float sphereCastRadius = 0.5f;

    private readonly RaycastHit[] hitBuffer = new RaycastHit[32];
    
    private List<HideObject> hiddenObjects = new List<HideObject>();
    private List<HideObject> previouslyhiddenObjects = new List<HideObject>();

    public GameObject tPlayer;
    
    private void LateUpdate()
    {
        RefreshHiddenObjects();
        
        if (!tPlayer.activeInHierarchy)
        {
            foreach (var hideable in previouslyhiddenObjects.Where(hideable => Vector3.Distance(transform.position, hideable.transform.position) >0.1f))
            {
                hideable.SetVisible(true);
            }
        }
        
    }


    private void RefreshHiddenObjects()
    {
        if (tPlayer == null)
        {
            if (tPlayer != null)
            {
                target = tPlayer.transform;
            }
            Debug.LogWarning("플레이어가 없습니다!");
            return;
        }

        var position = transform.position;
        var toTarget= target.position - position;
        var targetDistance = toTarget.magnitude;
        var targetDirection = toTarget / targetDistance;
        
        targetDistance -= sphereCastRadius * 1.1f;
        
        hiddenObjects.Clear();
        int hitCount= Physics.SphereCastNonAlloc(position, 
            sphereCastRadius, targetDirection, hitBuffer, targetDistance, 
            -1, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < hitCount; i++)
        {
            var hit = hitBuffer[i];
            var hideable = HideObject.GetHideObject(hit.collider);

            if (hideable != null)
                hiddenObjects.Add(hideable);
        }

        foreach (var hideable in hiddenObjects.Where(hideable => !previouslyhiddenObjects.Contains(hideable)))
        {
            hideable.SetVisible(false);
        }

        foreach (var hideable in previouslyhiddenObjects.Where(hideable => !hiddenObjects.Contains(hideable)))
        {
            hideable.SetVisible(true);
        }

        (hiddenObjects, previouslyhiddenObjects) = (previouslyhiddenObjects, hiddenObjects);
        
    }
}
