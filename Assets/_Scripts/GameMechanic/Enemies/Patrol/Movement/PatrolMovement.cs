using UnityEngine;

public class PatrolMovement : MonoBehaviour
{
    public Transform groundDetection;
    public float fSpeed;
    public float fStayFor;
    public float fTurnAroundFor;
    public float fMoveForMax;
    public bool facingRight = true;

    public Vector3 startRotation;
    public Vector3 endRotation;
    public bool detected = false;

    [HideInInspector]
    public float fMoveAt = 0;
    private float fStayAt = 0;
    
    private float groundInfoRayDistance = .2f;
    public LayerMask whatIsGround;

    [HideInInspector]
    public bool bStopMoving = false;
    [HideInInspector]
    public bool bMovingRight = true;


    private bool bTurnedAround = true;

    private FieldOfView fov;
    private Animator animator;

    private void Start()
    {
        fov = GetComponentInChildren<FieldOfView>();
        animator = GetComponent<Animator>();
        fMoveAt = Time.time;
        fStayAt = Time.time + fMoveForMax;
    }
    private void Update()
    {
        detected = fov.detected;
        HandleFieldOfView();
        if (!detected) HandleMovement();
        if (animator) animator.SetBool("staying", fMoveAt > Time.time || detected);
    }

    private void HandleMovement()
    {
        RaycastHit2D groundInfo = groundDetection 
            ? Physics2D.Raycast(
                groundDetection.position,
                Vector2.down,
                groundInfoRayDistance,
                whatIsGround
            )
            : Physics2D.Raycast(
                transform.position,
                Vector2.zero,
                0f,
                whatIsGround
            );
        if (bTurnedAround && !bStopMoving
            && (!groundInfo.collider || fStayAt < Time.time))
        {
            bStopMoving = true;
        }

        if (fMoveAt <= Time.time)
        {
            if (bTurnedAround)
            {
                if (facingRight) transform.Translate(Time.deltaTime * fSpeed * Vector2.right);
                else transform.Translate(Time.deltaTime * -fSpeed * Vector2.right);
            }
            else
            {
                bTurnedAround = true;
                fStayAt = Time.time + fMoveForMax;
            }
        }
    }

    private void HandleFieldOfView()
    {
        float facingParameter = Vector3.Dot(fov.transform.right, Vector3.right);
        bool shouldTurnAround = facingParameter >= 0 && !facingRight || facingParameter < 0 && facingRight;

        if (shouldTurnAround)
        {
            facingRight = !facingRight;
            transform.localScale = new Vector3(facingRight ? 1f: -1f, 1f, 1f);
            fov.transform.localScale = new Vector3(-fov.transform.localScale.x, fov.transform.localScale.y, fov.transform.localScale.z);
        }
        if (bStopMoving && fMoveAt < Time.time)
        {
            fMoveAt = Time.time + fStayFor + fTurnAroundFor;

            if (facingRight)
                StartCoroutine(GetComponentInChildren<FieldOfViewMovement>().RotateFromAngleToAngleForSecondsAfterSeconds(
                    startRotation,
                    endRotation,
                    fTurnAroundFor,
                    fStayFor - fTurnAroundFor
                ));
            else 
                StartCoroutine(GetComponentInChildren<FieldOfViewMovement>().RotateFromAngleToAngleForSecondsAfterSeconds(
                    endRotation,
                    startRotation,
                    fTurnAroundFor,
                    fStayFor - fTurnAroundFor
                ));
            bStopMoving = false;
            bTurnedAround = false;
        }


    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("SafeZone"))
        {
            GetComponent<Collider2D>().isTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("SafeZone"))
        {
            GetComponent<Collider2D>().isTrigger = false;
        }
    }
}
