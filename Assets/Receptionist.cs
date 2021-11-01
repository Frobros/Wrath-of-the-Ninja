using UnityEngine;

public class Receptionist : SecurityParent
{
    [SerializeField] private float tStayTime = 0f;
    [SerializeField] private float tStayFor = 0f;

    private SecurityLookAtPlayer lookAtPlayer;
    private Animator animator;
    private bool isFacingRight = true;

    private void Start()
    {
        lookAtPlayer = GetComponentInChildren<SecurityLookAtPlayer>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        HandleAnimation();
        if (!lookAtPlayer.IsBusy && !lookAtPlayer.IsTurning)
        {
            if (tStayTime >= tStayFor)
            {
                tStayTime = 0f;
                StartCoroutine(lookAtPlayer.TurnAround());
            }
            else
            {
                tStayTime += Time.deltaTime;
            }
        }
    }

    protected override void HandleAnimation()
    {
        animator.SetBool("isFacingRight", IsFacingRight());
    }

    public override bool IsFacingRight()
    {
        return isFacingRight;
    }

    public override void FaceRight(bool faceRight)
    {
        isFacingRight = faceRight;
    }
}
