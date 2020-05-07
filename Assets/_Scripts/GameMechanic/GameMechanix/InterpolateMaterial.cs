using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterpolateMaterial : MonoBehaviour
{
    private FieldOfView fieldOfView;
    private MeshRenderer renderer;
    public Material material1;
    public Material material2;

    bool interpolating;
    public float interpolateIn = 3f;
    private float interpolatingFor = 0f;
    public float lerp;

    void Start()
    {
        fieldOfView = GetComponentInParent<FieldOfView>();
        renderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        if (fieldOfView.detected)
        {
            lerp = Mathf.Min(lerp + Time.deltaTime / interpolateIn, 1f);
        } 
        else
        {
            lerp = Mathf.Max(lerp - Time.deltaTime / (0.5f * interpolateIn), 0f);
        }

        renderer.material.Lerp(material1, material2, lerp);
    }
}
