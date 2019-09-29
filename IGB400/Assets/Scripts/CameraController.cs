using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;

    public Vector3 offset;

    public float smoothSpeed;
    private bool _istargetNotNull;


    // Start is called before the first frame update
    private void Start()
    {
        _istargetNotNull = target != null;
    }

    private void FixedUpdate()
    {
        if (_istargetNotNull)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Slerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
        

        //transform.position = new Vector3();    
    }
}
