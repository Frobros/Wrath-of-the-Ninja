using System;
using UnityEditor.Rendering;
using UnityEngine;

public class PatrolMovement : MonoBehaviour
{
    public Transform groundDetection;
    public float fSpeed;
    public float fStayFor;
    public float fTurnAroundFor;
    public float fMoveForMax;

    [HideInInspector]
    public float fMoveAt = 0;
    private float fStayAt = 0;
    
    private float groundInfoRayDistance = .2f;
    public LayerMask whatIsGround;

    [HideInInspector]
    public bool bStopMoving = false;
    [HideInInspector]
    public bool bTurnedAround = true;
    [HideInInspector]
    public bool bMovingRight = true;

    private void Update()
    {
        RaycastHit2D groundInfo = Physics2D.Raycast(
            groundDetection.position,
            Vector2.down,
            groundInfoRayDistance,
            whatIsGround
        );

        if (bTurnedAround && !bStopMoving 
            && (!groundInfo.collider || fStayAt < Time.time))
        {
            bStopMoving = true;
        }

        if (bStopMoving && fMoveAt < Time.time)
        {
            fMoveAt = Time.time + fStayFor + fTurnAroundFor;
            StartCoroutine(GetComponentInChildren<PatrolTurnFaceContiously>().TurnAround(this));
            bStopMoving = false;
            bTurnedAround = false;
        }

        if (fMoveAt < Time.time)
        {
            if (!bTurnedAround)
            {
                bTurnedAround = true;
                fStayAt = Time.time + fMoveForMax;
            } 
            else 
            {
                transform.Translate(Time.deltaTime * fSpeed * Vector2.right);
            }
        }

        GetComponent<Animator>().SetBool("staying", fMoveAt > Time.time);
    }

    public void turnAround()
    {
        if (bMovingRight)
        {
            transform.eulerAngles = new Vector3(0, -180, 0);
            bMovingRight = false;
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            bMovingRight = true;
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
