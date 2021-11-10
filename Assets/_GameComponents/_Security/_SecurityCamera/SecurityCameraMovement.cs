using MyMath;
using System.Collections;
using UnityEngine;

public class SecurityCameraMovement : MonoBehaviour {
    [SerializeField] private InterpolateType interpolateType;
    [SerializeField] private float tStayFor;
    [SerializeField] private float tRotateFor;
    [SerializeField] float startRotationEulerZ, endRotationEulerZ;
    private float tStayTime;
    private float tRotateTime;

    private SecurityLookAtPlayer lookAtPlayer;
    private Quaternion startRotation, endRotation;
    private bool rotateForward = true;
    private bool isMoving;
    private bool wasInterrupted;

    private void Start()
    {
        startRotation = Quaternion.Euler(0, 0, startRotationEulerZ);
        endRotation = Quaternion.Euler(0, 0, endRotationEulerZ);
        transform.rotation = startRotation;
        lookAtPlayer = GetComponentInChildren<SecurityLookAtPlayer>();
    }

    void FixedUpdate() {
        if (!lookAtPlayer.IsBusy)
        {
            if (!isMoving)
            {
                if (rotateForward) StartCoroutine(RotateFromToInSeconds(startRotation, endRotation));
                else StartCoroutine(RotateFromToInSeconds(endRotation, startRotation));
            }
        }
    }

    private IEnumerator RotateFromToInSeconds(Quaternion from, Quaternion to)
    {
        isMoving = true;
        if (!wasInterrupted)
            tRotateTime = 0f;
        yield return new WaitUntil(() =>
        {
            tRotateTime += Time.deltaTime;
            transform.rotation = MyMath.Interpolation.Interpolate(from, to, tRotateTime / tRotateFor, interpolateType);
            return tRotateTime >= tRotateFor || lookAtPlayer.IsDetected;
        });
        if (!lookAtPlayer.IsDetected)
        {
            tStayTime = 0f;
            yield return new WaitUntil(() =>
            {
                tStayTime += Time.deltaTime;
                return tStayTime >= tStayFor || lookAtPlayer.IsDetected;
            });

            if (!lookAtPlayer.IsDetected)
            {
                rotateForward = !rotateForward;
                wasInterrupted = false;
            }
            else wasInterrupted = true;
        }
        else wasInterrupted = true;
        isMoving = false;
    }
}
