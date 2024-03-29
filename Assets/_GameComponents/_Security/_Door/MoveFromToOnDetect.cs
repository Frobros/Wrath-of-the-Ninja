﻿using MyMath;
using System.Collections;
using UnityEngine;

public class MoveFromToOnDetect : MonoBehaviour
{
    [SerializeField] private FieldOfView[] fovs;
    [SerializeField] private Vector2 fromLocalPosition;
    [SerializeField] private Vector2 toLocalPosition;
    [SerializeField] private InterpolateType interpolateType;
    [SerializeField] private bool isDetected;
    [SerializeField] private bool isStopped;
    [SerializeField] private bool isClosed;
    [SerializeField] private bool isOpen;
    [SerializeField] private float tOpeningFor = 1f;
    [SerializeField] private float tOpeningTime;
    [SerializeField] private float tClosingFor = 0.5f;
    [SerializeField] private float tClosingTime;
    [SerializeField] private float tOpenReactionOffsetFor = 0.5f;
    [SerializeField] private float tOpenReactionOffsetTime;
    [SerializeField] private float tClosedReactionOffsetFor = 0.5f;
    [SerializeField] private float tClosedReactionOffsetTime;
    private bool isClosing;
    private bool isOpening;

    void Start()
    {
        if (fovs.Length == 0)
            fovs = FindObjectsOfType<FieldOfView>();
        transform.localPosition = fromLocalPosition;
        isOpen = true;
    }

    void Update()
    {
        HandleDetection();
    }

    private void HandleDetection()
    {
        bool isCurrentlyDetected = false;
        foreach (FieldOfView fov in fovs)
        {
            if (fov.IsDetected)
            {
                isCurrentlyDetected = fov.IsDetected;
                break;
            }
        }
        isDetected = isCurrentlyDetected;
        if (isDetected && !isClosing && !isClosed)
        {
            StartCoroutine(CloseDoor());
        }
        if (!isDetected && !isOpening && !isOpen)
        {
            StartCoroutine(OpenDoor());
        }
    }

    private IEnumerator CloseDoor()
    {
        isClosing = true;
        tClosedReactionOffsetTime = 0f;
        yield return new WaitUntil(() =>
        {
            tClosedReactionOffsetTime += Time.deltaTime;
            return tClosedReactionOffsetTime >= tClosedReactionOffsetFor;
        });
        if (!isDetected) 
        {
            isClosing = false;
            yield break;
        }
        
        if (isOpen)
        {
            tClosingTime = 0f;
        }
        isOpen = false;
        yield return new WaitUntil(() =>
        {
            if (!isStopped)
            {
                tClosingTime += Time.deltaTime;
                tOpeningTime = (1f - tClosingTime / tClosingFor) * tOpeningFor;
                tOpeningTime -= (tClosingTime/ tClosingFor) * Time.deltaTime;
                transform.localPosition = Interpolation.Interpolate(fromLocalPosition, toLocalPosition, tClosingTime / tClosingFor, interpolateType);
            }
            return tClosingTime >= tClosingFor || !isDetected;
        });
        tOpeningTime = tOpeningTime >= 0f ? tOpeningTime : 0f;
        if (isDetected)
        {
            isClosed = true;
            transform.localPosition = toLocalPosition;
        }
        isClosing = false;
        Debug.Log("CLOSING ROUTINE END");
    }

    private IEnumerator OpenDoor()
    {
        Debug.Log("OPENING ROUTINE START");
        isOpening = true;
        tOpenReactionOffsetTime = 0f;
        yield return new WaitUntil(() =>
        {
            tOpenReactionOffsetTime += Time.deltaTime;
            return tOpenReactionOffsetTime >= tOpenReactionOffsetFor;
        });
        if (isDetected)
        {
            isOpening = false;
            yield break;
        }
        if (isClosed) tOpeningTime = 0f;

        isClosed = false;
        yield return new WaitUntil(() =>
        {
            if (!isStopped)
            {
                tOpeningTime += Time.deltaTime;
                tClosingTime = (1f - (tOpeningTime / tOpeningFor)) * tClosingFor;
                transform.localPosition = Interpolation.Interpolate(toLocalPosition, fromLocalPosition, tOpeningTime / tOpeningFor, interpolateType);
            }
            return tOpeningTime >= tOpeningFor || isDetected;
        });
        tClosingTime = tClosingTime >= 0f ? tClosingTime : 0f;
        if (!isDetected)
        {
            isOpen = true;
            transform.localPosition = fromLocalPosition;
        }
        isOpening = false;
        Debug.Log("OPENING ROUTINE END");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player" && tOpeningTime == 0f)
            isStopped = true;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
            isStopped = false;
    }
}
