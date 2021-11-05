﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Parallex : MonoBehaviour
{
    private float length, startpos;
    public Transform cam;
    public float parallexEffect;

    void Start()
    {
        cam = Camera.main.transform;
        startpos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float dist = (cam.position.x * parallexEffect);
        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);
    }
}
