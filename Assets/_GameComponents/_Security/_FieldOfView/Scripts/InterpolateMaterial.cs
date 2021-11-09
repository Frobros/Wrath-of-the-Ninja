using UnityEngine;

public class InterpolateMaterial : MonoBehaviour
{
    [SerializeField] private Material material1;
    [SerializeField] private Material material2;
    [SerializeField] private MyMath.InterpolateType interpolateType;
    private float tInterpolateFor = 3f;
    private float tInterpolateTime = 0f;
    private float interpolateFactor;
    private DetectedTimer timer;
    private MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        timer = GetComponent<DetectedTimer>();
        if (timer == null) timer = GetComponentInParent<DetectedTimer>();
    }

    void Update()
    {
        tInterpolateTime = timer.DetectedTime;
        tInterpolateFor = timer.DetectedFor;
        interpolateFactor = MyMath.InterpolateFunctions.Interpolate(0f, 1f, tInterpolateTime / tInterpolateFor, interpolateType);
        meshRenderer.material.Lerp(material1, material2, interpolateFactor);
    }
}
