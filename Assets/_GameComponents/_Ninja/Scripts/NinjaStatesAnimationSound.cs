using UnityEngine;

public class NinjaStatesAnimationSound : MonoBehaviour
{
    public Transform 
        groundCheckBack, 
        groundCheckFront,
        ledgeCheck,
        squeezeCheck, 
        wallCheckDown,
        wallCheckUp, 
        wedgedCheck;

    private Collider2D floor,
        ladderedDown,
        ladderedUp;
    private Rigidbody2D physicalBody;
    private new CapsuleCollider2D collider;
    private Animator animator;
    
    public LayerMask
        whatIsClimbableLeft,
        whatIsClimbableRight,
        whatIsGround,
        whatIsLedge,
        whatIsPermeable,
        whatIsSlippery;

    private float
        boxSize = 0.005f,
        collisionIgnoreFrame = 1F,
        revertCollisionIgnoreAt = 0F;

    public float
        wallJumpFrame = 0.3f,
        wallJumpEndAt = 0F,
        horizontal,
        vertical;

    private bool
        collisionIgnored = false;

    public bool
        climbing = false,
        controlHorizontal = true,
        dead = false,
        facingRight = true,
        groundedBack = false,
        groundedFront = false,
        grounded = false,
        hiddenFromRight = false,
        hiddenFromLeft = false,
        jumpButton,
        jumping = false,
        laddered,
        onLedge = false,
        onPermeableFloor = false,
        onClimbable = false,
        performLedgeClimb,
        performLedgeLand,
        shurikenButton,
        squeezed = false,
        wallJumping = false,
        walled = false, 
        walledDown = false, 
        walledUp = false, 
        wallGrab = false, 
        wedged = false,
        wasClimbingOnSameLadderBefore = false,
        ducking = false,
        landing = false,
        sliding = false,
        stairSliding = false,
        slipping = false;

    private ShotSprite shotSprite;

    void Start()
    {
        floor = null;
        collider = GetComponent<CapsuleCollider2D>();
        physicalBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        squeezeCheck.localPosition = new Vector2(collider.offset.x, collider.offset.y + 0.5f * collider.size.y);
        shotSprite = GetComponentInChildren<ShotSprite>();
    }

    internal bool isDetectable(Vector3 right)
    {
        float dotProduct = Vector3.Dot(Vector3.right, right);
        if (onClimbable && laddered &&
            (dotProduct <= 0 && ladderedDown.gameObject.layer == 10
            || dotProduct >= 0 && ladderedDown.gameObject.layer == 11))
        {
            return false;
        }
        return true;
    }

    private void Update()
    {
        HandleButtons();
        HandleAnimation();
        HandleSensors();
        HandlePermeableFloor();
    }


    private void FixedUpdate()
    {
        HandlePhysics();
        HandleStates();
        HandlePhysicalConstraints();
    }

    private void HandleButtons()
    {
        vertical = InputManager.yAxis;
        horizontal = Mathf.Abs(InputManager.xAxis) > 0.2f ? InputManager.xAxis : 0F;
        jumpButton = InputManager.jump;
        shurikenButton = InputManager.action;
    }

    internal bool isLedgeClimbingUp()
    {
        return performLedgeClimb;
    }

    internal bool isLedgeLanding()
    {
        return performLedgeLand;
    }

    private void HandleAnimation()
    {
        animator.SetBool("grounded", grounded);
        animator.SetFloat("horizontal", Mathf.Abs(horizontal));
        animator.SetFloat("vertical", vertical);
        animator.SetBool("onWall", climbing || wallGrab || sliding);
        animator.SetBool("wallGrab", wallGrab);
        animator.SetBool("climbing", climbing);
        animator.SetBool("ducking", ducking);
        animator.SetBool("onLedge", onLedge);
        animator.SetFloat("VeloY", physicalBody.velocity.y);
    }

    private void HandleSensors()
    {
        groundCheckFront.localPosition = new Vector2(collider.offset.x + 0.25f * collider.size.x, collider.offset.y - 0.5f * collider.size.y);
        groundCheckBack.localPosition = new Vector2(collider.offset.x - 0.25f * collider.size.x, collider.offset.y - 0.5f * collider.size.y);
        ledgeCheck.localPosition = new Vector2(collider.offset.x + 0.5f * collider.size.x, collider.offset.y + 0.25f * collider.size.y);
        wallCheckUp.localPosition = new Vector2(collider.offset.x + 0.5f * collider.size.x, collider.offset.y + 0.5f * collider.size.y - 0.5f * boxSize);
        wallCheckDown.localPosition = new Vector2(collider.offset.x + 0.5f * collider.size.x, collider.offset.y - 0.25f * collider.size.y);
        wedgedCheck.localPosition = new Vector2(collider.offset.x + 0.4375f * collider.size.x, collider.offset.y - 0.4375f * collider.size.y);
    }

    internal void resetLedgeClimb()
    {
        performLedgeClimb = false;
        performLedgeLand = false;
    }

    private void HandlePhysics()
    {
        groundedFront = Physics2D.OverlapBox(groundCheckFront.position, new Vector2(0.5f * collider.size.x - 0.015f, boxSize), 0.0f, whatIsGround);
        groundedBack = Physics2D.OverlapBox(groundCheckBack.position, new Vector2(0.5f * collider.size.x - 0.015f, boxSize), 0.0f, whatIsGround);
        if (!groundedFront || !groundedBack)
        {
            groundedFront = physicalBody.velocity.y <= 0f && Physics2D.OverlapBox(groundCheckFront.position, new Vector2(0.5f * collider.size.x - 0.015f, boxSize), 0.0f, whatIsPermeable);
            groundedBack = physicalBody.velocity.y <= 0f && Physics2D.OverlapBox(groundCheckBack.position, new Vector2(0.5f * collider.size.x - 0.015f, boxSize), 0.0f, whatIsPermeable);
        }
        grounded = groundedFront && groundedBack;


        LayerMask whatIsCurrentlyClimbable = facingRight ? whatIsClimbableLeft : whatIsClimbableRight;

        onLedge = Physics2D.OverlapBox(wallCheckUp.position, Vector2.right * boxSize + Vector2.up * 0.25f * collider.size.y, 0.0f, whatIsLedge);
        ladderedUp = Physics2D.OverlapBox(wallCheckUp.position, Vector2.right * boxSize + Vector2.up * 0.25f * collider.size.y, 0.0f, whatIsCurrentlyClimbable);
        ladderedDown = Physics2D.OverlapBox(wallCheckDown.position, Vector2.right * boxSize + Vector2.up * 0.25f * collider.size.y, 0.0f, whatIsCurrentlyClimbable);
        walledUp = !ladderedUp && !onLedge && Physics2D.OverlapBox(wallCheckUp.position, Vector2.right * boxSize + Vector2.up * 0.25f * collider.size.y, 0.0f, whatIsGround);
        walledDown = !ladderedDown && Physics2D.OverlapBox(wallCheckDown.position, Vector2.right * boxSize + Vector2.up * 0.25f * collider.size.y, 0.0f, whatIsGround);
        onPermeableFloor = Physics2D.OverlapBox(groundCheckFront.position, new Vector2(0.5f * collider.size.x - 0.015f, boxSize), 0.0f, whatIsPermeable)
            && Physics2D.OverlapBox(groundCheckBack.position, new Vector2(0.5f * collider.size.x - 0.015f, boxSize), 0.0f, whatIsPermeable);
        squeezed = Physics2D.OverlapBox(squeezeCheck.position, new Vector2(collider.size.x - 0.2f, boxSize), 0.0f, whatIsGround);
        wedged = Physics2D.OverlapBox(wedgedCheck.position, (Vector2.right * collider.size.x + Vector2.up * collider.size.y) * 0.0625f, 0.0f, whatIsGround);
        stairSliding = (!walledDown || !walledUp)
            && (Physics2D.OverlapBox(groundCheckFront.position, new Vector2(0.5f * collider.size.x - 0.015f, boxSize), 0.0f, whatIsSlippery) 
            || Physics2D.OverlapBox(groundCheckBack.position, new Vector2(0.5f * collider.size.x - 0.015f, boxSize), 0.0f, whatIsSlippery)
            || Physics2D.OverlapBox(wallCheckDown.position, Vector2.right * boxSize + Vector2.up * 0.25f * collider.size.y, 0.0f, whatIsSlippery));


        if (collisionIgnored && revertCollisionIgnoreAt < Time.time)
        {
            Physics2D.IgnoreCollision(collider, floor, false);
            floor = null;
            collisionIgnored = false;
        }
    }


    private void HandleStates()
    {
        facingRight = transform.localScale.x > 0.0f;
        ducking = grounded && (vertical < -0.7f || squeezed);

        if (onLedge && onClimbable) performLedgeClimb = true;
        else if (performLedgeClimb && !groundedFront)
        {
            physicalBody.velocity = Vector2.zero;
            performLedgeLand = true;
            performLedgeClimb = false;
        } else if (performLedgeLand && grounded) resetLedgeClimb();


        HandleClimbingStates();
        landing = groundedFront && !groundedBack && !walledDown && !walledUp;
        slipping = !groundedFront && groundedBack && !walledDown;
        controlHorizontal = !(wallGrab || climbing || sliding || stairSliding);

        if (wallJumping && wallJumpEndAt < Time.time)
        {
            wallJumping = false;
        }
    }

    private void HandleClimbingStates()
    {
        walled = walledUp && walledDown && !grounded;
        laddered = ladderedDown && ladderedUp && !grounded;

        if (walled && !onClimbable)
        {
            climbing = !wallGrab && vertical > 0.5f;
            onClimbable = climbing || physicalBody.velocity.y <= 0F;
        }
        else if (laddered && !onClimbable)
        {
            climbing = vertical > 0.5f;
            wasClimbingOnSameLadderBefore = climbing;
            onClimbable = wasClimbingOnSameLadderBefore;
        } else
        {
            onClimbable = walled || (laddered && wasClimbingOnSameLadderBefore);
            if (onClimbable)
            {
                wallGrab = commitAndGo() && vertical < 0.5f;
                climbing = vertical > 0.5f;
                sliding = !climbing && !wallGrab;
            } else
            {
                wallGrab = false;
                sliding = false;
                climbing = false;
                wasClimbingOnSameLadderBefore = false;
            }
        }
    }

    private void HandlePhysicalConstraints()
    {
        if (wallGrab)
        {
            physicalBody.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        else if (climbing || sliding)
        {
            physicalBody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }
        else
        {
            physicalBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }
    private void HandlePermeableFloor()
    {
        if (onPermeableFloor && ducking && jumpButton)
        {
            floor = Physics2D.OverlapBox(groundCheckFront.position, new Vector2(0.5f * collider.size.x - 0.015f, boxSize), 0.0f, whatIsPermeable);
            Physics2D.IgnoreCollision(collider, floor);
            revertCollisionIgnoreAt = Time.time + collisionIgnoreFrame;
            collisionIgnored = true;
        }
    }

    internal bool commitAndGo()
    {
        return facingRight && horizontal > 0F || !facingRight && horizontal < 0F;
    }

    internal void initialzeWallJump()
    {
        wallJumping = true;
        wallJumpEndAt = Time.time + wallJumpFrame;
    }

    internal void initialzeJump()
    {
        jumping = false;
    }

    internal bool isOnWall()
    {
        return climbing || sliding || wallGrab;
    }

    internal Collider2D isOnLadder()
    {
        return wasClimbingOnSameLadderBefore
            ? ladderedDown 
                ? ladderedDown 
                : ladderedUp 
                    ? ladderedUp 
                    : null
            : null;
    }

    public bool isDetectableFrom(Vector3 direction)
    {
        if (onClimbable)
        {
            return direction.x > 0 && facingRight || direction.x < 0 && !facingRight;
        }
        else return true;
    }

    internal void KillNinja()
    {
        dead = true;
    }

    internal void ShootNinja()
    {
        if (!dead)
        {
            KillNinja();
            FindObjectOfType<AudioManager>().PlaySound("shot", 1f);
            shotSprite.ShootNinja();
        }
    }
}
