﻿using UnityEngine;

public class HandleDoor : MonoBehaviour
{
    public Vector2 openLocalPosition;
    public Vector2 closedLocalPosition;

    public FieldOfView[] fovs;
    public bool detected;

    private bool stopped, closed;
    private float speed;
    private float closedAt;
    private float openedAt;
    public float openingIn = 1f;
    public float closingIn = 0.5f;

    void Start()
    {
        fovs = FindObjectsOfType<FieldOfView>();
    }

    void Update()
    {
        HandleDetection();
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (stopped)
        {
            closedAt += Time.deltaTime;
            openedAt += Time.deltaTime;
        }
        else 
        {
            if (closedAt > Time.time)
            {
                float deltaTime = (closingIn - (closedAt - Time.time)) / closingIn;
                transform.localPosition = Vector3.Lerp(openLocalPosition, closedLocalPosition, deltaTime);
                closed = true;
            }
            else if (openedAt > Time.time)
            { 
                float deltaTime = (openingIn - (openedAt - Time.time)) / openingIn;
                transform.localPosition = Vector3.Lerp(closedLocalPosition, openLocalPosition, deltaTime);
                closed = false;
            } else if (closed)
            {
                transform.localPosition = closedLocalPosition;
            } else
            {
                transform.localPosition = openLocalPosition;
            }
        }
    }

    private void HandleDetection()
    {
        bool currentlyDetected = false;
        foreach (FieldOfView fov in fovs)
        {
            if (fov.detected)
            {
                currentlyDetected = fov.detected;
            }
        }
        float halfWayThere;
        if (currentlyDetected && !detected)
        {
            if (openedAt > Time.time)
            {
                halfWayThere = (openingIn - (openedAt - Time.time)) / openingIn;
            }
            else
            {
                halfWayThere = 1f;
            }
            closedAt = Time.time + halfWayThere * closingIn;
            openedAt = Time.time;
        } 
        else if (!currentlyDetected && detected)
        {
            if (closedAt > Time.time)
            {
                halfWayThere = (closingIn - (closedAt - Time.time)) / closingIn;
            }
            else
            {
                halfWayThere = 1f;
            }
            openedAt = Time.time + halfWayThere * openingIn;
            closedAt = Time.time;
        }
        detected = currentlyDetected;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player" && openedAt < Time.time)
            stopped = true;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
            stopped = false;
    }
}