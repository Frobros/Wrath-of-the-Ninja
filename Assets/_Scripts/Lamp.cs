using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class Lamp : MonoBehaviour
{
    Light2D light;
    PolygonCollider2D polygon;

    private void Start()
    {
        light = GetComponent<Light2D>();
        polygon = gameObject.GetComponent<PolygonCollider2D>();
        polygon.points = new[] {
            (Vector2) light.shapePath[0],
            (Vector2) light.shapePath[1],
            (Vector2) light.shapePath[2],
            (Vector2) light.shapePath[3]
        };
        polygon.isTrigger = true;
    }

}
