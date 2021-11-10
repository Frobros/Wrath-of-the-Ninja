using UnityEngine;

public class DetectedTimer : MonoBehaviour
{
    [SerializeField] private float tDetectedFor;
    [SerializeField] private bool canBeShot;
    private FieldOfView fov;
    private float tDetectedTime;

    public float DetectedTime { get { return tDetectedTime; } }
    public float DetectedFor { get { return tDetectedFor; } }


    void Start()
    {
        fov = GetComponent<FieldOfView>();
        if (fov == null)
            fov = GetComponentInChildren<FieldOfView>();
    }

    void Update()
    {
        if (fov.IsDetected)
        {
            tDetectedTime += Time.deltaTime;
            tDetectedTime = Mathf.Min(tDetectedTime, tDetectedFor);
        }
        else
        {
            tDetectedTime -= Time.deltaTime;
            tDetectedTime = Mathf.Max(tDetectedTime, 0f);
        }

        if (tDetectedTime >= tDetectedFor)
        {
            if (canBeShot)
                FindObjectOfType<NinjaStatesAnimationSound>().ShootNinja();
        }
    }
}
