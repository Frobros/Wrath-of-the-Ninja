using System;
using System.Collections;
using UnityEngine;

public class SecurityCameraMovement : MonoBehaviour {
    public float fStayFor;
    public float fRotateFor;

    private float fStayUntil;
    private float fRotateUntil;

    public Vector3 startRotationEuler, endRotationEuler;
    private Quaternion startRotation, endRotation;
    private bool rotateBackwards = true;
    private bool staying;

    public bool detected = false;
    private bool wasDetected = false;
    private bool backToNormal = true;
    private bool rotationInterrupted = false;
    private Quaternion formerRotation;

    private void Start()
    {
        startRotation = Quaternion.Euler(startRotationEuler);
        endRotation = Quaternion.Euler(endRotationEuler);
        transform.rotation = startRotation;
    }

    void FixedUpdate() {
        detected = GetComponentInChildren<FieldOfView>().detected;
        if (detected)
            HandleDetectionRotation();
        else
            HandleStandardRotation();
    }

    private void HandleDetectionRotation()
    {
        if (!wasDetected)
        {
            rotationInterrupted = true;
        }

        Vector2 up = Vector2.Perpendicular(FindObjectOfType<NinjaMove>().transform.position - transform.position);
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.25f);

        wasDetected = true;
        backToNormal = false;
        fStayUntil += Time.deltaTime;
        fRotateUntil += Time.deltaTime;
    }

    private void HandleStandardRotation()
    {
        if (wasDetected)
        {
            wasDetected = false;
            formerRotation = RotationCloserToCurrent(startRotation, endRotation);
            rotationInterrupted = true;
            StartCoroutine(RotateFromToInSeconds(transform.rotation, formerRotation, 1f, 0.5f));
        } else if (!backToNormal)    
        {
            fStayUntil += Time.deltaTime;
            fRotateUntil += Time.deltaTime;
        }
        else
        {
            if (!staying && fRotateUntil < Time.time)
            {
                fStayUntil = Time.time + fStayFor;
                staying = true;
            } 
            else if (staying && fStayUntil < Time.time)
            {
                rotationInterrupted = true;
                staying = false;
                rotateBackwards = !rotateBackwards;
                fRotateUntil = Time.time + fRotateFor;

                if (rotateBackwards) StartCoroutine(RotateFromToInSeconds(endRotation, startRotation, fRotateFor, 0f));
                else StartCoroutine(RotateFromToInSeconds(startRotation, endRotation, fRotateFor, 0f));
            }
        }
    }

    private Quaternion RotationCloserToCurrent(Quaternion startRotation, Quaternion endRotation)
    {
        Vector3 currentLookVector = transform.rotation * Vector3.right;
        Vector3 startLookVector = startRotation * Vector3.right;
        Vector3 endLookVector = endRotation * Vector3.right;

        float startDot = Vector3.Dot(currentLookVector, startLookVector);
        float endDot = Vector3.Dot(currentLookVector, endLookVector);

        if (startDot > endDot)
        {
            rotateBackwards = true;
            return startRotation;
        }
        else
        {
            rotateBackwards = false;
            return endRotation;
        }

    }

    private IEnumerator RotateFromToInSeconds(Quaternion from, Quaternion to, float inSeconds, float afterSeconds)
    {
        yield return new WaitForSeconds(afterSeconds);
        rotationInterrupted = false;
        float delta;
        float rotateUntil = Time.time + inSeconds;
        bool rotating = true;
        while (rotating && !rotationInterrupted)
        {
            delta = (inSeconds - (rotateUntil - Time.time)) / inSeconds;
            transform.rotation = Quaternion.Lerp(from, to, delta);
            rotating = Time.time < rotateUntil;
            yield return null;
        }
        backToNormal = true;
    }
}
