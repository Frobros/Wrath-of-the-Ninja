using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewMovement : MonoBehaviour
{
    private bool turnAround = false;
    private float currentTime;
    private float forSeconds;
    Quaternion from, to;
    private bool detected;

    void Update()
    {
        detected = GetComponent<FieldOfView>().detected;
        if (turnAround && !detected)
        {
            transform.rotation = transform.parent.rotation * Quaternion.Lerp(from, to, currentTime);
            currentTime += Time.deltaTime / forSeconds;
        }
    }

    public IEnumerator RotateFromAngleToAngleForSecondsAfterSeconds(Vector3 fromEulerAngles, Vector3 toEulerAngles, float forSeconds, float afterSeconds)
    {
        yield return new WaitForSeconds(afterSeconds);
        currentTime = 0f;
        from = Quaternion.Euler(fromEulerAngles);
        to = Quaternion.Euler(toEulerAngles);
        this.forSeconds = forSeconds;
        turnAround = true;
        yield return new WaitUntil(() => currentTime >= 1f);
        turnAround = false;
        currentTime = 0f;
    }
}
