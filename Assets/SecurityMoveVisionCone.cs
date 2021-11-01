using UnityEngine;


public class SecurityMoveVisionCone : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float tResetIn;
    [SerializeField] private bool isReset = true;
    private float tResetTime;
    private Quaternion reference;
    private Transform player;
    private FieldOfView fov;

    public bool IsFacingRight { get { return IsPatrol ? transform.parent.localScale.x > 0f : transform.localScale.x > 0f; } }
    public bool IsPatrol { get { return transform.GetComponentInParent<SecurityWalkBackAndForth>() != null; } }
    void Start()
    {
        player = FindObjectOfType<NinjaStatesAnimationSound>().transform;
        fov = GetComponent<FieldOfView>();
    }

    void Update()
    {
        if (fov.IsDetected && !fov.IsTurning)
        {
            if (isReset)
            {
                reference = transform.rotation;
                isReset = false;
            }
            else
            {
                // Turn around if necessary
                if (IsPatrol)
                {
                    float facingDirecton = transform.right.x;
                    Debug.Log(transform.right);
                    if (facingDirecton < 0f) {
                        transform.parent.localScale = new Vector3(
                            -transform.parent.localScale.x,
                            transform.parent.localScale.y,
                            transform.parent.localScale.z
                        );
                    }
                }

                // Look at player
                Vector2 playerDirection = (player.position - transform.position);
                playerDirection = Vector3.Normalize(
                    IsFacingRight ? playerDirection : -playerDirection
                );
                transform.right = MyMath.InterpolateFunctions.Interpolate((Vector2)transform.right, playerDirection, speed * Time.deltaTime);
            }
        }
        else if (!isReset)
        {
            tResetTime += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(transform.rotation, reference, tResetTime / tResetIn);
            isReset = tResetTime >= tResetIn;
        }
    }
}
