using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBound : MonoBehaviour
{
    public List<Bounds> bounds;
    public float cameraSize;

    void Start()
    {
        if (cameraSize != 0f) Camera.main.orthographicSize = cameraSize;
        bounds = new List<Bounds>();
        foreach (BoxCollider2D collider in GetComponents<BoxCollider2D>())
        {
            bounds.Add(collider.bounds);
        }
    }
}
