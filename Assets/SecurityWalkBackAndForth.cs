using UnityEngine;

public class SecurityWalkBackAndForth : SecurityParent
{
    [SerializeField] private GameObject groundCheck;
    [SerializeField] private float startXPosition;
    [SerializeField] private float endXPosition;
    [SerializeField] private float tMoveFor;
    [SerializeField] private float tStayFor;
    [SerializeField] private float tTurnFor;
    [SerializeField] private MyMath.InterpolateType interpolateType;

    private FieldOfView fov;
    private Animator animator;
    private Rigidbody2D rb;
    private float tMoveAt = 0f;
    private float tStayAndTurnTime = 0f;
    private bool isWalkingForth = true;
    private bool isStaying = false;

    internal float StayForSeconds { get { return tStayFor; } }
    internal float TurnForSeconds { get { return tTurnFor; } }
    private void Start()
    {
        fov = GetComponentInChildren<FieldOfView>();
        rb = GetComponent<Rigidbody2D>();
        transform.position = new Vector2(startXPosition, transform.position.y);
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (fov.IsReset)
        {
            IncrementTime();
            HandleMovement();
        }
        HandleAnimation();
    }

    private void IncrementTime()
    {
        if (!isStaying && (isWalkingForth && tMoveAt > tMoveFor || !isWalkingForth && tMoveAt < 0f))
        {
            isStaying = true;
            StartCoroutine(fov.TurnAroundForSecondsAfterSeconds(tTurnFor, tStayFor));
        }
        else if (isStaying)
        {
            if (tStayAndTurnTime < (tStayFor + tTurnFor))
            {
                tStayAndTurnTime += Time.deltaTime;
            } else
            {
                tStayAndTurnTime = 0f;
                isStaying = false;
                isWalkingForth = !isWalkingForth;
            }
        }
        else if (isWalkingForth)
        {
            tMoveAt += Time.deltaTime;
        } 
        else {
            tMoveAt -= Time.deltaTime;
        }
    }


    private void HandleMovement()
    {
        float time = tMoveFor != 0f ? tMoveAt / tMoveFor : 0;
        Vector2 targetPosition = new Vector2(
            MyMath.InterpolateFunctions.Interpolate(startXPosition, endXPosition, time, interpolateType),
            transform.position.y
        );
        rb.MovePosition(targetPosition);
    }

    private void HandleAnimation()
    {
        animator.SetBool("staying", isStaying);
        transform.localScale = new Vector3(
            ShallFaceRight()
                ? Mathf.Abs(transform.localScale.x)
                : -Mathf.Abs(transform.localScale.x),
            transform.localScale.y,
            transform.localScale.z
        );
    }
    private bool ShallFaceRight()
    {
        return startXPosition < endXPosition && transform.localScale.x > 0f
            || startXPosition > endXPosition && transform.localScale.x < 0f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            float facingDirection = collision.transform.position.x - transform.position.x > 0f
                ? Mathf.Abs(transform.localScale.x)
                : -Mathf.Abs(transform.localScale.x);

            transform.localScale = new Vector3(
                facingDirection,
                transform.localScale.y,
                transform.localScale.z
            );
        }
    }
}