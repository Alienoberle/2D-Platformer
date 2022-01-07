using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;

/*
 * Basic set up of the script
 * https://youtu.be/MbWK8bCAU2w?list=PLFt_AvWsXl0f0hqURlhyIoAabKPgRsqjz
 */

public class PlayerController : MonoBehaviour
{
    private PlayerCollision playerCollision;
    [HideInInspector] public Rigidbody2D rb2D;
    private Animator animator;

    private Vector2 directionalInput;
    [HideInInspector] public Vector2 magneticVelocity;
    [HideInInspector] public Vector2 platformVelocity;
    [SerializeField] private Vector2 velocity;
    private float deltaTime;
    [HideInInspector] public PlayerInfo playerInfo;
    [SerializeField] private Vector2 aimInput;

    [Header("GroundCheck")]
    private float gravity;
    [SerializeField] private float maxGravity = -9.0f;
    public bool isGravity = true;
    
    [Header("Movement")]
    public float movementSpeed = 6;
    private float velocityXSmoothing;
    private float accelerationTimeAirborn = 0.2f;
    private float accelerationTimeGrounded = 0.1f;
    //[SerializeField] private float maximumSlopeAngle = 60.0f;

    [Header("Jumping")]
    [SerializeField] private float maxJumpHeight = 4f;
    [SerializeField] private float minJumpHeight = 1f;
    [SerializeField] private float timeToJumpApex = 0.25f;

    [SerializeField] private float maxJumpVelocity;
    [SerializeField] private float minJumpVelocity;

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

    [Header("Events")]
    [Space]
    public UnityEvent OnLandEvent;
    public UnityEvent OnJumpEvent;
    public UnityEvent OnDashEvent;
    public UnityEvent OnMagnetismEvent;
    [System.Serializable] public class BoolEvent : UnityEvent<bool> { }


    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        playerCollision = GetComponent<PlayerCollision>();
        animator = GetComponent<Animator>();
        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();
    }

    private void Start()
    {
        playerInfo.Initialize();
        ResetJump();
        ResetDash();

        gravity = -(2 * maxJumpHeight / Mathf.Pow(timeToJumpApex, 2));
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
    }
    private void FixedUpdate()
    {
        deltaTime = Time.fixedDeltaTime;
        UpdatePlayerInfo();
        Gravity();
        CalculateInputVelocity();
        WallSliding();              
        Dashing();
        GhostJump();

        // Hand over the input and calcualted velocity to the playercontroller handling the actual movement and collision
        Move(velocity * deltaTime);

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
    }
    private void CalculateInputVelocity()
    {
        // Calculate the target X velocity based on input and movement speed. 
        float targetVelocityX = directionalInput.x * movementSpeed;

        //only control the player if grounded or airControl is turned on
        if (playerInfo.isGrounded || playerInfo.hasAirControl)
        {
            // Reduce horizontal velocity when airborne
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (playerInfo.isGrounded) ? accelerationTimeGrounded : accelerationTimeAirborn);
        }
    }
    public void Move(Vector2 aVelocity)
    {
        rb2D.velocity = aVelocity * 50.0f;
        rb2D.velocity += (playerInfo.isStandingOnPlatform)? platformVelocity: Vector2.zero;
        rb2D.velocity += (playerInfo.isMagnetismActive)? magneticVelocity : Vector2.zero;
    }
    private void Gravity()
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
    private void UpdatePlayerInfo() // coudl use clean up currentyl double book keeping in PlayerCollision and here
    {
        playerInfo.wasGrounded = playerInfo.isGrounded;
        playerInfo.isGrounded = playerCollision.collisionInfo.isGrounded;
        playerInfo.isStandingOnPlatform = playerCollision.collisionInfo.isStandingOnPlatform;

        if(playerInfo.isGrounded && !playerInfo.wasGrounded)
        {
            OnLandEvent.Invoke();
            ResetJump();
            ResetDash();
        }
    }
    private void GhostJump()
    {
        if (ghostJumpTimer > 0 && ghostJumpActive)
        {
            ghostJumpTimer -= deltaTime;
        }

        if (playerInfo.wasGrounded && !playerInfo.isGrounded)
        {
            ghostJumpActive = true;
        }

        if (ghostJumpTimer < 0)
        {
            ghostJumpActive = false;
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
    public void OnJumpInput()
    {     
        // If the player is currently wall sliding
        if (playerInfo.isWallsliding)
        {
            if (wallDirectionX == Mathf.Sign(directionalInput.x))
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

        if (CanPlayerJump())
        {
            if (directionalInput.y !> -0.95f) // input values are from 0.999... to -0.999...
            {
                velocity.y = maxJumpVelocity;
                Debug.Log(maxJumpVelocity);
            }

            jumpCounter -= 1;
            playerInfo.isJumping = true;
            animator.SetBool("IsJumping", true);
            OnJumpEvent.Invoke();
        }

    }
    public void OnJumpInputRelease()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }
    private bool CanPlayerJump()
    {
        if (jumpCounter <= jumpAmount && jumpCounter > 0)
        {
            if (playerInfo.isGrounded || playerInfo.isWallsliding || ghostJumpActive)
            {
                return true;
            }
            return false;
        }
        return false;
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
        dashVelocity = dashDistance / dashDuration;
        if (CanPlayerDash())
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
            OnDashEvent.Invoke();
        }
    }
    private bool CanPlayerDash()
    {
        if(dashCounter > 0)
        {
            return true;
        }
        return false;
    }
    private void Dashing()
    {
        if (playerInfo.isDashing)
        {
            isGravity = false;
            velocity.y = 0;
            velocity.x = dashVelocity * dashingDirectionX;
            dashTimer -= deltaTime;

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

                if (playerInfo.isGrounded)
                    ResetDash();
            }
        }
    }
    public void ResetDash()
    {
        dashCounter = dashAmount;
        animator.SetBool("IsDashing", false);
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
    public void OnChangeCharge(Polarization newPolarization)
    {
        switch (newPolarization)
        {
            case Polarization.negative:
                playerInfo.isMagnetismActive = true;
                playerInfo.hasAirControl = false;

                break;
            case Polarization.positive:
                playerInfo.isMagnetismActive = true;
                playerInfo.hasAirControl = false;
                break;
            case Polarization.neutral:
                playerInfo.isMagnetismActive = false;
                playerInfo.hasAirControl = true;
                break;
        }
    }
    public struct PlayerInfo
    {
        public bool isGrounded { get; set; }
        public bool wasGrounded { get; set; }
        public bool hasAirControl { get; set; }
        public bool isStandingOnPlatform { get; set; }
        public bool isMagnetismActive { get; set; }
        public bool isWallsliding { get; set; }
        public bool isJumping { get; set; }
        public bool isDashing { get; set; }

        public int facingDirection;

        public void Initialize()
        {
            isGrounded = true;
            wasGrounded = true;
            hasAirControl = true;
            isMagnetismActive = false;
            isStandingOnPlatform = false;
            isWallsliding = false;
            isJumping = false;
            isDashing = false;
            facingDirection = 1;
        }
    }
}
