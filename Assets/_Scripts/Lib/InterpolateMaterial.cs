using UnityEngine;

public class InterpolateMaterial : MonoBehaviour
{
    [SerializeField] private Material material1;
    [SerializeField] private Material material2;
    [SerializeField] private bool canBeShot;
    [SerializeField] private float interpolateIn = 3f;
    [SerializeField] private float lerp;
    
    private FieldOfView fov;
    private MeshRenderer meshRenderer;

    void Start()
    {
        fov = GetComponentInParent<FieldOfView>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        if (fov.IsDetected)
        {
            lerp = Mathf.Min(lerp + Time.deltaTime / interpolateIn, 1f);
        } 
        else
        {
            lerp = Mathf.Max(lerp - Time.deltaTime / (0.5f * interpolateIn), 0f);
        }
        meshRenderer.material.Lerp(material1, material2, lerp);

        if (lerp == 1f && canBeShot)
        {
            FindObjectOfType<NinjaStatesAnimationSound>().ShootNinja();
        }
    }
}
