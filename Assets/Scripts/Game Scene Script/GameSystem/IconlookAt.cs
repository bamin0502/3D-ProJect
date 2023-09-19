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
        var transform1 = _cam.transform;
        Vector3 lookAtPosition = transform1.position;
        lookAtPosition.x = transform.position.x;
        transform.LookAt(lookAtPosition,transform1.up);
    }
}
