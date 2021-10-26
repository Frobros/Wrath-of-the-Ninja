using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class Lamp : MonoBehaviour
{
    [HideInInspector]
    public int lightId;

    private static int numberOfLights = 0;
    UnityEngine.Experimental.Rendering.Universal.Light2D light;
    PolygonCollider2D polygon;

    private void Start()
    {
        light = GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>();
        polygon = gameObject.AddComponent<PolygonCollider2D>();
        List<Vector2> points = new List<Vector2>();
        foreach (Vector2 point in light.shapePath)
        {
            points.Add(point);
        }
        polygon.points = points.ToArray();
        polygon.isTrigger = true;
        lightId = numberOfLights;
        numberOfLights++;
    }

}
