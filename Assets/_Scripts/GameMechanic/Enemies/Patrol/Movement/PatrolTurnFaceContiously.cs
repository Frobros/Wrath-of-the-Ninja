using System.Collections;
using UnityEngine;

public class PatrolTurnFaceContiously : MonoBehaviour
{

    public IEnumerator TurnAround(PatrolMovement patrolMovement)
    {
        float currentAngle = patrolMovement.bMovingRight 
            ? 0f
            : 180f;
        float angleIncreasePerSec = patrolMovement.bMovingRight 
            ? 180f / patrolMovement.fStayFor
            : -180f / patrolMovement.fStayFor;
        float perpendicularAt = Time.time + .5f * patrolMovement.fStayFor;
        float rotatedAt = Time.time + patrolMovement.fStayFor;

        while (Time.time < perpendicularAt)
        {
            currentAngle += Time.deltaTime * angleIncreasePerSec;
            transform.eulerAngles = new Vector3(0, currentAngle, 0);
            yield return null;
        }
        patrolMovement.turnAround();
        while (Time.time < rotatedAt)
        {
            currentAngle += Time.deltaTime * angleIncreasePerSec;
            transform.eulerAngles = new Vector3(0, currentAngle, 0);
            yield return null;
        }

        transform.eulerAngles = patrolMovement.bMovingRight
            ? new Vector3(0, 0, 0)
            : new Vector3(0, 180, 0);
    }
}
