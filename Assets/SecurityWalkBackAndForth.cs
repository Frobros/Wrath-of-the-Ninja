using UnityEngine;
using System;

public class SecurityWalkBackAndForth : MonoBehaviour
{
    [SerializeField] private GameObject groundCheck;
    [SerializeField] private float startXPosition;
    [SerializeField] private float endXPosition;
    [SerializeField] private float tMoveFor;
    [SerializeField] private float tStayFor;
    [SerializeField] private MyMath.InterpolateType interpolateType;

    private FieldOfView fov;
    private Rigidbody2D rb;
    private float tMoveAt = 0f;
    private float tStayAt = 0f;
    private bool isWalkingForth = true;
    private bool isStaying = false;

    public bool IsStaying { get { return isStaying; } }
    public bool IsFacingRight
    {
        get {
            return startXPosition < endXPosition && isWalkingForth
                || startXPosition > endXPosition && !isWalkingForth;
        }
    }

    private void Start()
    {
        fov = GetComponentInChildren<FieldOfView>();
        rb = GetComponent<Rigidbody2D>();
        transform.position = new Vector2(startXPosition, transform.position.y);
    }

    private void Update()
    {
        if (!fov.detected)
        {
            IncrementTime();
            HandleMovement();
        }
    }

    private void IncrementTime()
    {
        if (!isStaying && (isWalkingForth && tMoveAt > tMoveFor || !isWalkingForth && tMoveAt < 0f))
        {
            isStaying = true;
        }
        else if (isStaying)
        {
            if (tStayAt < tStayFor)
            {
                tStayAt += Time.deltaTime;
            } else
            {
                tStayAt = 0f;
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
        float time = tMoveAt / tMoveFor;
        Vector2 targetPosition = new Vector2(
            MyMath.InterpolateFunctions.Interpolate(startXPosition, endXPosition, time, interpolateType),
            transform.position.y
        );
        rb.MovePosition(targetPosition);
    }
}
