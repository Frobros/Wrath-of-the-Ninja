using UnityEngine;

public class NinjaMove : MonoBehaviour {
    private Rigidbody2D physicalBody;
    private Animator anim;
    private NinjaStatesAnimationSound state;
    
    private Vector2 wallJumpDir = Vector2.zero;
    private float
        slidingSpeed = 3f,
        translation = 5.0f;
    public float
        ledgeJumpHeight = 5F,
        ledgeLandingSpeed = 5F,
        horizontalDamping = 0.5f,
        fallMultiplier = 2.5f,
        lowJumpMultiplier = 0.5f,
        timeScale = 0.8f,
        climbingSpeed = 3F,
        jumpHeight = 3.5f,
        walkingSpeed = 5F,
        wallJumpSpeed = 3F,
        wallJumpHeight = 3F;
    private float fJumpPressedRemember = 0;
    private float fGroundedRemember = 0;
    public float 
        fJumpPressedRememberTime = 0.5f,
        fGroundedRememberTime;
    

    // Jump And Fall Debugging
    private GameObject line;
    private int lineIndex = 0;
    private float fTryToPassPermeableFloorRemember = 0f;
    private float fTryToPassPermeableFloorRememberTime = 0.25f;

    void Start()
    {
        physicalBody = GetComponent<Rigidbody2D>();
        state = GetComponent<NinjaStatesAnimationSound>();
        Time.timeScale = timeScale;
    }

    private void Update()
    {
        HandleJumpAndFall();
    }

    private void FixedUpdate()
    {
        // DebugJumpAndFall();

        if (state.wallJumpEndAt < Time.time)
        {
            changeFacing();
            if (state.controlHorizontal) HandleMovement();
            HandleClimbing();
        }
    }


    void DebugJumpAndFall()
    {
        if (!state.grounded)
        {
            if (lineIndex == 0)
            {
                if (line == null)
                {
                    line = new GameObject("line");
                    line.AddComponent<LineRenderer>();
                }
                line.GetComponent<LineRenderer>().startWidth = 0.25f;
                line.GetComponent<LineRenderer>().endWidth = 0.25f;
                lineIndex = 0;
            }
            line.GetComponent<LineRenderer>().positionCount = lineIndex + 1;
            line.GetComponent<LineRenderer>().SetPosition(lineIndex, transform.position);
            lineIndex++;
        }
        else if (state.grounded && lineIndex != 0)
        {
            lineIndex = 0;
        }
    }


    private void HandleMovement()
    {
        if (!state.walled)
        {
            float horizontalVelocity = walkingSpeed 
                * (state.ducking ? 0.5f : 1F)
                * (InputManager.xAxis > 0.2f 
                    ? 1F 
                    : InputManager.xAxis < -0.2f 
                        ? -1F
                        : 0F
                );
            physicalBody.velocity = Vector3.Lerp(physicalBody.velocity, new Vector2(horizontalVelocity, physicalBody.velocity.y), 10F);
        }
        if (state.landing)
        {
            physicalBody.AddForce(new Vector3(state.facingRight ? translation : -translation, 0F, 0F), ForceMode2D.Force);
        } else if(state.wedged && !state.groundedFront && !state.walled && physicalBody.velocity.y == 0.0f)
        {
            physicalBody.velocity = new Vector2(state.facingRight ? -3.5f : 3.5f, 0.0f);
        }
    }

    public void HandleClimbing()
    {
        if (state.isLedgeClimbingUp())
        {
            physicalBody.velocity = new Vector2(0F, ledgeJumpHeight);
        }
        else if (state.isLedgeLanding())
        {
            physicalBody.velocity = new Vector2(state.facingRight ? ledgeLandingSpeed : -ledgeLandingSpeed, physicalBody.velocity.y); 
        }
        if (state.climbing)
        {
            physicalBody.velocity = new Vector2(0.0f, climbingSpeed);
        }
        if (state.sliding && physicalBody.velocity.y < -slidingSpeed)
        {
            physicalBody.velocity = new Vector2(0F, -slidingSpeed);
        }
    }

    private void HandleJumpAndFall() {

        fGroundedRemember -= Time.deltaTime;
        fTryToPassPermeableFloorRemember -= Time.deltaTime;
        bool tryToPassPermeableFloor = state.onPermeableFloor && InputManager.down && InputManager.jump;

        if (tryToPassPermeableFloor) {
            fTryToPassPermeableFloorRemember = fTryToPassPermeableFloorRememberTime;
        }
        else if (fTryToPassPermeableFloorRemember < 0f)
        {

            if (state.grounded)
            {
                fGroundedRemember = fGroundedRememberTime;

            }
            fJumpPressedRemember -= Time.deltaTime;
            if (state.jumpButton)
            {
                fJumpPressedRemember = fJumpPressedRememberTime;
            }

            if (fJumpPressedRemember > 0 && fGroundedRemember > 0 && !state.ducking)
            {
                fJumpPressedRemember = 0F;
                fGroundedRemember = 0F;
                physicalBody.velocity = new Vector2(physicalBody.velocity.x, jumpHeight);
            }

            if (state.jumpButton && state.isOnWall())
            {
                HandleWallJump();
            }
            else if (state.wallJumpEndAt > Time.time)
            {
                changeFacingBasedOnVelocity();
                float deltaTime = (state.wallJumpEndAt - Time.time) / state.wallJumpFrame;
                float horizontalVelocity = Mathf.Lerp(state.horizontal * walkingSpeed, wallJumpDir.x, deltaTime);
                
                physicalBody.velocity = new Vector2(horizontalVelocity, physicalBody.velocity.y);
                /*
                float horizontalVelocity = state.horizontal * walkingSpeed;
                horizontalVelocity = Mathf.SmoothDamp(physicalBody.velocity.x, horizontalVelocity, ref velocityX, 10F*horizontalDamping);
                physicalBody.velocity = new Vector2(horizontalVelocity, physicalBody.velocity.y);
                */
            }
        }

        if (!InputManager.jumpContinuous && physicalBody.velocity.y > 0F)
        {
            physicalBody.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1F) * Time.deltaTime;
        }

        if (physicalBody.velocity.y < 0F && !state.grounded)
        {
            physicalBody.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1F) * Time.deltaTime;

            // physicalBody.velocity = new Vector2(physicalBody.velocity.x, physicalBody.velocity.y * fallMultiplier);
        }
    }

    public void HandleWallJump()
    {
        TurnDirection();
        wallJumpDir = new Vector2(isFacingRight() ? wallJumpSpeed : -wallJumpSpeed, wallJumpHeight);
        physicalBody.velocity = Vector2.zero;
        Debug.Log(physicalBody.velocity);
        transform.Translate(state.facingRight ? 0.1f * Vector2.right : -0.1f * Vector2.right);
        physicalBody.velocity = wallJumpDir;
        state.initialzeWallJump();
    }

    private bool isFacingRight()
    {
        return transform.localScale.x > 0f;
    }

    private void changeFacingBasedOnVelocity()
    {
        if ((state.facingRight && physicalBody.velocity.x < 0F)
               || !state.facingRight && physicalBody.velocity.x > 0F)
        {
            TurnDirection();
        }
    }

    void changeFacing()
    {
        if ((state.facingRight && state.horizontal < 0F)
            || !state.facingRight && state.horizontal > 0F)
        {
            if (!state.climbing || state.climbing && Mathf.Abs(state.horizontal) > 0.5f)
                TurnDirection();
        }
    }

    private void TurnDirection()
    {
        if (state.isLedgeClimbingUp() || state.isLedgeLanding()) {
            state.resetLedgeClimb();
        }
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }
}