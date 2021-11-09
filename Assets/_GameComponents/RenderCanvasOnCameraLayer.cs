using UnityEngine;

public class RenderCanvasOnCameraLayer : MonoBehaviour
{
    void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
    }
}
