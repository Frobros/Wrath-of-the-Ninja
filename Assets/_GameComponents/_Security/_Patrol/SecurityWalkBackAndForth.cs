using MyMath;
using System.Collections;
using UnityEngine;

public class SecurityWalkBackAndForth : MonoBehaviour
{
    [SerializeField] private GameObject groundCheck;
    [SerializeField] private InterpolateType interpolateType;
    [SerializeField] private float startXPosition;
    [SerializeField] private float endXPosition;
    [SerializeField] private float tMoveFor;
    [SerializeField] private float tStayFor;
    [SerializeField] private float tMoveTime = 0f;
    [SerializeField] private float tStayTime = 0f;
    [SerializeField] private bool shallWalkForth = true;
    [SerializeField] private bool isWalking = false;
    [SerializeField] private bool isStaying = true;
    [SerializeField] private bool isInWalkingRoutine = false;

    private Patrol parent;
    private Rigidbody2D rb;
    private SecurityLookAtPlayer lookAtPlayer;

    public bool IsStaying { get { return isStaying; } }

    private void Start()
    {
        lookAtPlayer = GetComponentInChildren<SecurityLookAtPlayer>();
        rb = GetComponent<Rigidbody2D>();
        transform.position = new Vector2(startXPosition, transform.position.y);
        parent = GetComponent<Patrol>();
        parent.FaceRight(startXPosition < endXPosition);
    }

    private void Update()
    {
        if (!lookAtPlayer.IsBusy && !isInWalkingRoutine)
        {
            if (shallWalkForth) StartCoroutine(MoveFromTo(startXPosition, endXPosition));
            else StartCoroutine(MoveFromTo(endXPosition, startXPosition));
        }
    }

    private IEnumerator MoveFromTo(float startPosition, float endPosition)
    {
        isInWalkingRoutine = true;
        if (!isWalking)
        {
            tMoveTime = 0f;
            tStayTime = 0f;
            isWalking = true;
        }
        if (tMoveTime < tMoveFor)
        {
            isStaying = false;
            yield return new WaitUntil(() =>
            {
                tMoveTime += Time.deltaTime;
                Vector2 targetPosition = new Vector2(
                    Interpolation.Interpolate(startPosition, endPosition, tMoveTime / tMoveFor, interpolateType),
                    transform.position.y
                );
                rb.MovePosition(targetPosition);
                return tMoveTime >= tMoveFor || lookAtPlayer.IsDetected;
            });
            isStaying = true;
        }

        if (lookAtPlayer.IsDetected)
        {
            isInWalkingRoutine = false;
            yield break;
        }
        if (tStayTime < tStayFor)
        {
            yield return new WaitUntil(() => {
                tStayTime += Time.deltaTime;
                return tStayTime >= tStayFor || lookAtPlayer.IsDetected;
            });
        }

        if (lookAtPlayer.IsDetected)
        {
            isInWalkingRoutine = false;
            yield break;
        }

        shallWalkForth = !shallWalkForth;
        isWalking = false;
        isInWalkingRoutine = false;
        StartCoroutine(lookAtPlayer.TurnAround());
    }
}