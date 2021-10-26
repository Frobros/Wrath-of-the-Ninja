using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class Lamp : MonoBehaviour
{
    UnityEngine.Experimental.Rendering.Universal.Light2D light;
    PolygonCollider2D polygon;

    private void Start()
    {
        light = GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>();
        polygon = gameObject.AddComponent<PolygonCollider2D>();
        polygon.SetPath(0, new[] {
            (Vector2) light.shapePath[0],
            (Vector2) light.shapePath[1],
            (Vector2) light.shapePath[2]
        });
        polygon.isTrigger = true;
    }

}
