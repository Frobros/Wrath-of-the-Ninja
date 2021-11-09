using UnityEngine;

public class SecurityCamera : SecurityParent
{
    public override bool IsFacingRight() { return transform.right.x > 0f; }
}
