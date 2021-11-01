using UnityEngine;

public class InterpolateMaterial : MonoBehaviour
{
    [SerializeField] private Material material1;
    [SerializeField] private Material material2;
    [SerializeField] private bool canBeShot;
    [SerializeField] private float tInterpolateFor = 3f;
    [SerializeField] private float tInterpolateTime = 0f;
    [SerializeField] private float lerp;
    
    private FieldOfView fov;
    private MeshRenderer meshRenderer;
    private MyMath.InterpolateType interpolateType;

    void Start()
    {
        fov = GetComponentInParent<FieldOfView>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        if (fov.IsDetected)
        {
            tInterpolateTime += Time.deltaTime;
        } 
        else
        {
            tInterpolateTime -= Time.deltaTime;
        }
        tInterpolateTime = Mathf.Clamp(tInterpolateTime, 0f, tInterpolateFor);
        lerp = MyMath.InterpolateFunctions.Interpolate(0f, 1f, tInterpolateTime / tInterpolateFor, interpolateType);
        meshRenderer.material.Lerp(material1, material2, lerp);

        if (lerp >= 1f && canBeShot)
        {
            FindObjectOfType<NinjaStatesAnimationSound>().ShootNinja();
        }
    }
}
