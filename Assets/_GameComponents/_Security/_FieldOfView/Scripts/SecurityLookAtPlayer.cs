using System;
using System.Collections;
using UnityEngine;


enum TurnType
{
    TURN_AROUND_ANIMATE,
    TURN_AROUND_AXIS
}
public class SecurityLookAtPlayer : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float tResetFor;
    [SerializeField] private float tTurnFor;
    [SerializeField] private float tWaitBeforeResetFor;
    [SerializeField] private bool isReset = true;
    [SerializeField] private bool wasFacingRightBeforeDetect;
    [SerializeField] private bool isInTurningRoutine;
    [SerializeField] private bool wasTurningBeforeDetect;
    [SerializeField] private bool hasTurnedHalf;
    [SerializeField] private bool isInResetingRoutine;
    [SerializeField] private float tResetTime;
    [SerializeField] private float tTurnTime;

    private Transform player;
    private FieldOfView fov;
    private SecurityParent parent;
    private TurnType turnType;
    private Quaternion resetReferenceRotation;

    public bool IsTurning { get { return isInTurningRoutine; } }
    public bool IsReset { get { return isReset; } }
    public bool IsDetected { get { return fov.IsDetected; } }
    public bool IsBusy { get { return IsTurning || IsDetected || !IsReset; } }

    private void Start()
    {
        player = FindObjectOfType<NinjaStatesAnimationSound>().transform;
        fov = GetComponent<FieldOfView>();
        isReset = true;
        wasFacingRightBeforeDetect = false;
        parent = GetComponentInParent<SecurityParent>();
        if (parent == null) parent = GetComponent<SecurityParent>();
        turnType = typeof(Patrol) == parent.GetType() ? TurnType.TURN_AROUND_AXIS : TurnType.TURN_AROUND_ANIMATE;
    }

    private void Update()
    {
        HandleLookDirection();
    }

    private void HandleLookDirection()
    {
        if (IsDetected && !isInTurningRoutine)
        {
            if (isReset)
            {
                isReset = false;
                wasFacingRightBeforeDetect = parent.IsFacingRight();
                if (!wasTurningBeforeDetect)
                    resetReferenceRotation = transform.rotation;
            }
            // Look at player
            Vector2 playerDirection = Vector3.Normalize(player.position - transform.position);
            if (turnType == TurnType.TURN_AROUND_AXIS && !parent.IsFacingRight())
            {
                playerDirection = -playerDirection;
            }
            transform.right = MyMath.InterpolateFunctions.Interpolate((Vector2) transform.right, playerDirection, speed * Time.deltaTime);
            // Turn around if necessary
            if (turnType == TurnType.TURN_AROUND_AXIS)
            {
                if (transform.right.x <= 0)
                {
                    parent.FaceRight(!parent.IsFacingRight());
                    transform.right = new Vector2(-transform.right.x, transform.right.y);
                }
            }
            else
            {
                parent.FaceRight(transform.right.x > 0);
            }
        }
        else if (!IsDetected && !isReset && !isInResetingRoutine)
        {
            StartCoroutine(ResetLookDirection());
        }
    }

    private IEnumerator ResetLookDirection()
    {
        isInResetingRoutine = true;
        float tWaitBeforeResetTime = 0f;
        yield return new WaitUntil(() => {
            tWaitBeforeResetTime += Time.deltaTime;
            return tWaitBeforeResetTime >= tWaitBeforeResetFor || IsDetected;
        });
        if (IsDetected)
        {
            isInResetingRoutine = false;
            yield break;
        }
        tResetTime = 0f;
        yield return new WaitUntil(() =>
        {
            tResetTime += Time.deltaTime;
            transform.rotation = MyMath.InterpolateFunctions.Interpolate(transform.rotation, resetReferenceRotation, tResetTime / tResetFor);

            if (turnType == TurnType.TURN_AROUND_AXIS)
            {
                if (transform.right.x <= 0)
                {
                    parent.FaceRight(!parent.IsFacingRight());
                }
            }
            else
            {
                parent.FaceRight(transform.right.x > 0);
            }
            return tResetTime >= tResetFor || IsDetected;
        });
        if (IsDetected)
        {
            isInResetingRoutine = false;
            yield break;
        }

        if (turnType == TurnType.TURN_AROUND_AXIS)
        {

            if (!wasTurningBeforeDetect 
                && (wasFacingRightBeforeDetect != parent.IsFacingRight())
                || wasTurningBeforeDetect 
                && (
                    !hasTurnedHalf && (wasFacingRightBeforeDetect == parent.IsFacingRight())
                    || hasTurnedHalf && (wasFacingRightBeforeDetect != parent.IsFacingRight())
                )
            )
            {
                StartCoroutine(TurnAround());
            }
        }
        yield return new WaitUntil(() => !isInTurningRoutine || IsDetected);
        if (!IsDetected)
        {
            isReset = true;
            wasTurningBeforeDetect = false;
            hasTurnedHalf = false;
        }
        isInResetingRoutine = false;
    }

    public IEnumerator TurnAround()
    {
        isInTurningRoutine = true;
        tTurnTime = 0f;
        bool facingRight = parent.IsFacingRight();

        Quaternion fromRotation, toRotation;
        if (turnType == TurnType.TURN_AROUND_AXIS)
        {
            fromRotation = transform.rotation;
            toRotation = facingRight ? Quaternion.Euler(0f, 90f, 0f) : Quaternion.Euler(0f, -90f, 0f);
        }
        else
        {
            fromRotation = facingRight ? Quaternion.Euler(0f, 0f, 0f) : Quaternion.Euler(0f, 180f, 0f);
            toRotation = Quaternion.Euler(0f, 90f, 0f);
        }
        float turnForHalf = tTurnFor / 2f;
        yield return new WaitUntil(() =>
        {
            tTurnTime += Time.deltaTime;
            transform.rotation = MyMath.InterpolateFunctions.Interpolate(fromRotation, toRotation, tTurnTime / turnForHalf);
            return tTurnTime >= turnForHalf || IsDetected;
        });
        if (IsDetected)
        {
            resetReferenceRotation = fromRotation;
            wasFacingRightBeforeDetect = parent.IsFacingRight();
            wasTurningBeforeDetect = true;
            isInTurningRoutine = false;
            hasTurnedHalf = false;
            yield break;
        }
        transform.rotation = toRotation;

        if (turnType == TurnType.TURN_AROUND_AXIS)
        {
            toRotation = fromRotation;
            fromRotation = facingRight ? Quaternion.Euler(0f, -90f, 0f) : Quaternion.Euler(0f, 90f, 0f);
        }
        else
        {
            tTurnTime = 0f;
            fromRotation = toRotation;
            toRotation = facingRight ? Quaternion.Euler(0f, 180f, 0f) : Quaternion.Euler(0f, 0f, 0f);
        }

        parent.FaceRight(!facingRight);
        hasTurnedHalf = true;
        tTurnTime = 0f;
        yield return new WaitUntil(() =>
        {
            tTurnTime += Time.deltaTime;
            transform.rotation = MyMath.InterpolateFunctions.Interpolate(fromRotation, toRotation, tTurnTime / turnForHalf);
            return tTurnTime >= turnForHalf || IsDetected;
        });
        if (IsDetected)
        {
            resetReferenceRotation = toRotation;
            wasTurningBeforeDetect = true;
            isInTurningRoutine = false;
            wasFacingRightBeforeDetect = parent.IsFacingRight();
            yield break;
        }
        transform.rotation = toRotation;
        isInTurningRoutine = false;
        wasTurningBeforeDetect = false;
    }
}
