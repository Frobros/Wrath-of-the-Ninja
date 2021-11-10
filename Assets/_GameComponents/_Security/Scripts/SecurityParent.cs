public class SecurityParent : UnityEngine.MonoBehaviour
{
    public virtual bool IsFacingRight() { return false; }
    protected virtual void HandleAnimation() { }
    public virtual void FaceRight(bool faceRight) { }
}
