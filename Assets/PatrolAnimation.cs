using UnityEngine;

public class PatrolAnimation : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    private SecurityWalkBackAndForth walk;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        walk = GetComponent<SecurityWalkBackAndForth>();
    }

    private void Update()
    {
        animator.SetBool("staying", walk.IsStaying);

        float facingDirection = walk.IsFacingRight
            ? Mathf.Abs(transform.localScale.x)
            : -Mathf.Abs(transform.localScale.x);
        transform.localScale = new Vector3(
            facingDirection,
            transform.localScale.y,
            transform.localScale.z
        );
    }
}
