using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconlookAt : MonoBehaviour
{
    public Camera _cam;

    private void Awake()
    {
        // 프리팹이 인스턴스화될 때 미니맵용 카메라를 찾아 할당합니다.
        _cam = GameObject.FindGameObjectWithTag("MiniMap Camera").GetComponent<Camera>();
        // "MiniMapCamera"는 미니맵용 카메라에 할당된 태그입니다. 필요에 따라 태그를 변경해야합니다.
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 lookAtPosition = _cam.transform.position;
        lookAtPosition.x = transform.position.x;
        transform.LookAt(lookAtPosition);
    }
}
