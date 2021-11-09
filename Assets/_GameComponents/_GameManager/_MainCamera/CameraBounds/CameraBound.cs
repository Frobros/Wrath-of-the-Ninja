using System.Collections.Generic;
using UnityEngine;

public class CameraBound : MonoBehaviour
{
    [SerializeField] private float cameraSize;
    [SerializeField] private List<Bounds> bounds;

    public Bounds[] Bounds { get { return bounds.ToArray();  } }

    void Awake()
    {
        if (cameraSize != 0f) Camera.main.orthographicSize = cameraSize;
        bounds = new List<Bounds>();
        foreach (BoxCollider2D collider in GetComponents<BoxCollider2D>())
        {
            bounds.Add(collider.bounds);
        }
    }
}
