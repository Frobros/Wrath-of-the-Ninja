using MyMath;
using System.Collections;
using UnityEngine;

public class SecurityCameraMovement : SecurityParent {
    [SerializeField] private InterpolateType interpolateType;
    [SerializeField] private float tStayFor;
    [SerializeField] private float tRotateFor;
    [SerializeField] float startRotationEulerZ, endRotationEulerZ;
    private float tStayTime;
    private float tRotateTime;

    private FieldOfView fov;
    private Quaternion startRotation, endRotation;
    private bool rotateForward = true;
    private bool isMoving;
    private bool wasInterrupted;

    private void Start()
    {
        startRotation = Quaternion.Euler(0, 0, startRotationEulerZ);
        endRotation = Quaternion.Euler(0, 0, endRotationEulerZ);
        transform.rotation = startRotation;
        fov = GetComponentInChildren<FieldOfView>();
    }

    void FixedUpdate() {
        if (fov.IsReset)
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
            transform.rotation = MyMath.InterpolateFunctions.Interpolate(from, to, tRotateTime / tRotateFor, interpolateType);
            return tRotateTime >= tRotateFor || fov.IsDetected;
        });
        if (!fov.IsDetected)
        {
            tStayTime = 0f;
            yield return new WaitUntil(() =>
            {
                tStayTime += Time.deltaTime;
                return tStayTime >= tStayFor || fov.IsDetected;
            });

            if (!fov.IsDetected)
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
