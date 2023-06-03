using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconlookAt : MonoBehaviour
{
    public Camera _cam;

    private void Awake()
    {
        _cam = GameObject.FindGameObjectWithTag("MiniMap Camera").GetComponent<Camera>();
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 lookAtPosition = _cam.transform.position;
        lookAtPosition.x = transform.position.x;
        transform.LookAt(lookAtPosition,_cam.transform.up);
    }
}
