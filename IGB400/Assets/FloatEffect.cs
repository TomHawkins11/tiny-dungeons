using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatEffect : MonoBehaviour
{
    
    public float amplitude = 0.5f;
    public float frequency = 1f;
 

    Vector2 posOffset = new Vector2 ();
    Vector2 tempPos = new Vector2 ();
 
    // Use this for initialization
    void Start () {
        // Store the starting position & rotation of the object
        posOffset = transform.position;
    }
     
    // Update is called once per frame
    void Update () {

        // Float up/down with a Sin()
        tempPos = posOffset;
        tempPos.y += Mathf.Sin (Time.fixedTime * Mathf.PI * frequency) * amplitude;
 
        transform.position = tempPos;
    }
}
