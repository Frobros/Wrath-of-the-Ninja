using UnityEngine;

public class Patrol : SecurityParent
{
    private SecurityWalkBackAndForth movement;
    private Animator animator;

    private void Start()
    {
        movement = GetComponentInChildren<SecurityWalkBackAndForth>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        HandleAnimation();
    }

    protected override void HandleAnimation()
    {
        animator.SetBool("staying", movement.IsStaying);
    }

    public override void FaceRight(bool faceRight)
    {
        transform.localScale = new Vector3(
            faceRight ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x),
            transform.localScale.y,
            transform.localScale.z
        );
    }
}
