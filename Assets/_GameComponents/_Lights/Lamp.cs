using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Lamp : MonoBehaviour
{
    [HideInInspector]
    public int lightId;

    private static int numberOfLights = 0;
    private Light2D lampLight;
    PolygonCollider2D polygon;

    private void Start()
    {
        lampLight = GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>();
        polygon = GetComponent<PolygonCollider2D>();
        if (polygon == null)
        {
            polygon = gameObject.AddComponent<PolygonCollider2D>();
            List<Vector2> points = new List<Vector2>();
            foreach (Vector2 point in lampLight.shapePath)
            {
                points.Add(point);
            }
            polygon.points = points.ToArray();
            polygon.isTrigger = true;
        }
        lightId = numberOfLights;
        numberOfLights++;
    }

}
