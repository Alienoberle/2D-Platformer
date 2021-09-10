using UnityEngine;
using System.Collections;

/*
 * Basic set up of the script
 * https://youtu.be/MbWK8bCAU2w?list=PLFt_AvWsXl0f0hqURlhyIoAabKPgRsqjz
 */

[RequireComponent(typeof(PlayerCollision))]
public class PlayerController : MonoBehaviour
{
    PlayerCollision playerCollision;
    MagneticObject magneticComponent;

    [HideInInspector] public PlayerInfo playerInfo;
    [HideInInspector] public Vector2 directionalInput;
    public Vector2 magneticForce;
    public Vector2 velocity;
    private float deltaTime;

    [SerializeField] private Animator animator;

    [Header("Movement")]
    public float movementSpeed = 6;
    private float velocityXSmoothing;
    private float accelerationTimeAirborn = 0.2f;
    private float accelerationTimeGrounded = 0.1f;
    [SerializeField] private float maximumSlopeAngle = 60.0f;

    private float gravity;
    private float maxGravity = -20.0f;
    public bool isGravity = true;

    [Header("Jumping")]
    [SerializeField] private float maxJumpHeight = 4f;
    [SerializeField] private float minJumpHeight = 1;
    [SerializeField] private float timeToJumpApex = 0.25f;

    private float maxJumpVelocity;
    private float minJumpVelocity;

    public int jumpAmount = 1;
    private int jumpCounter;

    [SerializeField] private float ghostJumpTime = 0.1f;
    private float ghostJumpTimer;
    private bool ghostJumpActive;

    [Header("Dashing")]
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float dashDuration = 0.2f;
    private float dashVelocity;
    private float dashTimer;
    private float dashProgress = 0.5f;
    public int dashAmount = 1;
    private int dashCounter;
    private int dashingDirectionX;

    [Header("Wall Jump")]
    [SerializeField] private Vector2 wallJumpClimb;
    [SerializeField] private Vector2 wallJumpOff;
    [SerializeField] private Vector2 wallLeap;
    [Header("Wall Sliding")]
    [SerializeField] private float wallSlideSpeedMax = 3.0f;
    [SerializeField] private float wallStickTime = 0.25f;
    private float timeToWallUnstick;
    private int wallDirectionX;

    [Header("Magnetism")]
    [SerializeField] GameObject magnetVisualization;


    private void Awake()
    {
        playerCollision = GetComponent<PlayerCollision>();
        playerCollision.maxSlopeAngle = maximumSlopeAngle;
        magneticComponent = GetComponent<MagneticObject>();
    }

    private void Start()
    {
        gravity = -(2 * maxJumpHeight / Mathf.Pow(timeToJumpApex, 2));
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight); // check video for info https://www.youtube.com/watch?v=rVfR14UNNDo

        dashVelocity = dashDistance / dashDuration;
    
        playerInfo.Reset();
    }

    // Update is called once per frame
    private void Update()
    {
        // Calculate velocities
        deltaTime = Time.deltaTime;
        CalculateGravity();
        CalculateMagneticForce();
        CalculateInputVelocity();   
        WallSliding();              
        Dashing();


        // Hand over the input and calcualted velocity to the playercontroller handling the actual movement and collision
        playerCollision.Move(velocity * deltaTime, directionalInput);

        // Handle jumping
        IsPlayerGrounded();
        GhostJump();

        // Flip the players faceing direction if needed
        if (directionalInput.x > 0 && playerInfo.facingDirection < 0 )
        {
            FlipPlayer();
        }
        if (directionalInput.x < 0 && playerInfo.facingDirection > 0)
        {
            FlipPlayer();
        }

        // Trigger Run animation
        animator.SetFloat("Speed", Mathf.Abs(velocity.x));

        // Reset gravity to 0 to avoid gravity amassing when simply standing around. We want to do this after we have moved the player with our input because platforms also move the player
        if (playerCollision.collisionInfo.above || playerCollision.collisionInfo.below)
        {
            if (playerCollision.collisionInfo.slidingDownMaxSlope)
            {
                velocity.y += playerCollision.collisionInfo.slopeNormal.y * -gravity * deltaTime; // oppose velocity y by gravity over time 
            }
            else
            {
                velocity.y = 0;
            }
        }
    }
    private void CalculateGravity()
    {
        if (isGravity)
        {
            if (velocity.y + gravity * deltaTime < maxGravity)
                velocity.y = maxGravity;
            else
                velocity.y += gravity * deltaTime;
        }
        else
        {
            velocity.y = 0;
        }
    }
    private void CalculateInputVelocity()
    {
        // Calculate the target X velocity based on input and movement speed. 
        float targetVelocityX = directionalInput.x * movementSpeed;

        // Reduce horizontal velocity when airborne
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (playerCollision.collisionInfo.below) ? accelerationTimeGrounded : accelerationTimeAirborn);
    }
    private void CalculateMagneticForce()
    {
        if (magneticForce.x != 0 || magneticForce.y != 0)
        {
            velocity.x = magneticForce.x;
            velocity.y = magneticForce.y;
        }
    } 
    private void WallSliding()
    {
        // Figure out wall direction
        wallDirectionX = (playerCollision.collisionInfo.left) ? -1 : 1;

        playerInfo.isWallsliding = false;
        if ((playerCollision.collisionInfo.left || playerCollision.collisionInfo.right) && !playerCollision.collisionInfo.below && velocity.y < 0)
        {
            // Set wallSlideSpeedMax
            playerInfo.isWallsliding = true;
            if (velocity.y < -wallSlideSpeedMax)
            {
                velocity.y = -wallSlideSpeedMax;
            }

            if (timeToWallUnstick > 0)
            {
                velocity.x = 0;
                velocityXSmoothing = 0;

                if (directionalInput.x != wallDirectionX && directionalInput.x != 0)
                {
                    timeToWallUnstick -= deltaTime;
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else
            {
                timeToWallUnstick = wallStickTime;
            }
        }
    }
    private void IsPlayerGrounded()
    {
        if (playerCollision.collisionInfo.below)
        {
            playerInfo.isGrounded = true;
            ResetJump();
            ResetDash();
        }
        else
        {
            playerInfo.isGrounded = false;
        }
    }
    private void GhostJump()
    {
        if (ghostJumpTimer > 0 && ghostJumpActive)
        {
            ghostJumpTimer -= deltaTime;
        }

        if (playerCollision.collisionInfo.lastFrameBelow == true && playerCollision.collisionInfo.below == false)
        {
            ghostJumpActive = true;
        }

        if (ghostJumpTimer < 0)
        {
            ghostJumpActive = false;
        }
    }
    public void OnJumpInput()
    {
        CanPlayerJump();
        // If the player is currently wall sliding
        if (playerInfo.isWallsliding)
        {
            if (wallDirectionX == directionalInput.x)
            {
                velocity.x = -wallDirectionX * wallJumpClimb.x;
                velocity.y = wallJumpClimb.y;
            }
            else if (directionalInput.x == 0)
            {
                velocity.x = -wallDirectionX * wallJumpOff.x;
                velocity.y = wallJumpOff.y;
            }
            else
            {
                velocity.x = -wallDirectionX * wallLeap.x;
                velocity.y = wallLeap.y;
            }
        }

        if (playerInfo.canJump)
        {
            if (playerCollision.collisionInfo.slidingDownMaxSlope)
            {
                if (directionalInput.x != -Mathf.Sign(playerCollision.collisionInfo.slopeNormal.x)) // if we are not jumping against max slope normal
                {
                    velocity.y = maxJumpVelocity * playerCollision.collisionInfo.slopeNormal.y;
                    velocity.x = maxJumpVelocity * playerCollision.collisionInfo.slopeNormal.x;

                }
            }

            else if (directionalInput.y != -1)
            {
                velocity.y = maxJumpVelocity;
            }

            jumpCounter -= 1;
            playerInfo.isJumping = true;
            animator.SetBool("IsJumping", true);
        }

    }
    public void OnJumpInputRelease()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }
    private void CanPlayerJump()
    {
        playerInfo.canJump = false;
        if (jumpCounter > 0)
        {
            if (playerInfo.isGrounded)
            {
                playerInfo.canJump = true;
            }
            else if (jumpAmount > 1)
            {
                playerInfo.canJump = true;
            }
            else if (playerInfo.isWallsliding)
            {
                playerInfo.canJump = true;
            }
            else if (ghostJumpActive)
            {
                playerInfo.canJump = true;
            }
        }
    }
    public void ResetJump()
    {
        playerInfo.isJumping = false;
        jumpCounter = jumpAmount;
        animator.SetBool("IsJumping", false);

        ghostJumpActive = false;
        ghostJumpTimer = ghostJumpTime;
    }
    public void OnDashInput()
    {
        CanPlayerDash();
        dashVelocity = dashDistance / dashDuration;
        if (playerInfo.canDash)
        {
            if (directionalInput.x != 0)
            {
                dashingDirectionX = (int)Mathf.Sign(directionalInput.x);
            }
            else
            {
                dashingDirectionX = playerInfo.facingDirection;
            }
            dashCounter -= 1;
            playerInfo.isDashing = true;
            animator.SetBool("IsDashing", true);
        }
    }
    private void CanPlayerDash()
    {
        playerInfo.canDash = false;
        if(dashCounter > 0)
        {
            playerInfo.canDash = true;
        }
    }
    private void Dashing()
    {
        if (playerInfo.isDashing)
        { 
            velocity.y = 0;
            velocity.x = dashVelocity * dashingDirectionX;
            dashTimer -= deltaTime;
            isGravity = false;

            if ((dashTimer * dashProgress) < 0)
            {
                float targetVelocityX = directionalInput.x * movementSpeed;
                velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, dashTimer * (1-dashProgress));
                dashTimer -= deltaTime;
            }
            if(dashTimer < 0)
            {
                dashTimer = dashDuration;
                playerInfo.isDashing = false;
                isGravity = true;
            }
        }
    }
    public void ResetDash()
    {
        playerInfo.isDashing = false;
        dashCounter = dashAmount;
        animator.SetBool("IsDashing", false);
    }
    public void OnChangeCharge(Polarization newPolarization)
    {
        magneticComponent.ChangePolarisation(newPolarization);
        switch (newPolarization)
        {
            case Polarization.negative:
                magnetVisualization.SetActive(true);
                magnetVisualization.GetComponent<SpriteRenderer>().color = Color.red;
                break;
            case Polarization.positive:
                magnetVisualization.SetActive(true);
                magnetVisualization.GetComponent<SpriteRenderer>().color = Color.blue;
                break;
            case Polarization.neutral:
                magnetVisualization.SetActive(false);
                break;
        }
    }
    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }
    void FlipPlayer()
    {
        playerInfo.facingDirection = (int)Mathf.Sign(directionalInput.x);

        // Multiply the player's x local scale by -1.
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
    }
    public struct PlayerInfo
    {
        public bool isGrounded { get; set; }
        public bool isWallsliding { get; set; }
        public bool isJumping { get; set; }
        public bool isDashing { get; set; }
        public bool canJump { get; set; }
        public bool canDash { get; set; }

        public int facingDirection;

        public void Reset()
        {
            isGrounded = true;
            isWallsliding = false;
            isJumping = false;
            isDashing = false;
            canJump = true;
            canDash = true;
            facingDirection = 1;
        }
    }
}
