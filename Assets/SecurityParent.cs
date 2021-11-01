public class SecurityParent : UnityEngine.MonoBehaviour
{
    public virtual bool IsFacingRight() { return transform.localScale.x > 0f; }
    protected virtual void HandleAnimation() { }
    public virtual void FaceRight(bool faceRight) { }
}
