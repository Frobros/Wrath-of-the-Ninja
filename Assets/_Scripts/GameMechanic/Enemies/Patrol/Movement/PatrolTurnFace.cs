using System.Collections;
using UnityEngine;

public class PatrolTurnFace : MonoBehaviour
{
    public IEnumerator TurnAround(PatrolMovement patrolMovement)
    {
        yield return new WaitForSeconds(patrolMovement.fStayFor);
        float currentAngle = patrolMovement.bMovingRight 
            ? 0f
            : 180f;
        float angleIncreasePerSec = patrolMovement.bMovingRight 
            ? 180f / patrolMovement.fTurnAroundFor
            : -180f / patrolMovement.fTurnAroundFor;
        float perpendicularAt = Time.time + .5f * patrolMovement.fTurnAroundFor;
        float rotatedAt = Time.time + patrolMovement.fTurnAroundFor;

        while (Time.time < perpendicularAt)
        {
            currentAngle += Time.deltaTime * angleIncreasePerSec;
            transform.eulerAngles = new Vector3(0, currentAngle, 0);
            yield return null;
        }
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
