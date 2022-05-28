using System;
using UnityEngine;
using UnityEngine.Events;

/*
 * Basic set up of the script
 * https://youtu.be/MbWK8bCAU2w?list=PLFt_AvWsXl0f0hqURlhyIoAabKPgRsqjz
 */

public class PlayerController : MonoBehaviour
{
    private PlayerCollision playerCollision;
    [HideInInspector] public Rigidbody2D rb2D;
    private Animator animator;
    private AudioManager audioManager;

    public Vector2 directionalInput { get; private set; }
    [HideInInspector] public Vector2 magneticVelocity;
    [HideInInspector] public Vector2 platformVelocity;
    [SerializeField] private Vector2 velocity;
    private float deltaTime;
    [HideInInspector] public PlayerInfo playerInfo;

    [Header("Gravity")]
    private float gravity;
    [SerializeField] private float maxGravity = -9.0f;
    public bool isGravity = true;
    
    [Header("Movement")]
    public float movementSpeed = 6;
    private float velocityXSmoothing;
    private float accelerationTimeAirborn = 0.2f;
    private float accelerationTimeGrounded = 0.1f;
    [SerializeField] private ParticleSystem runVFX;
    [SerializeField] private AudioClip runSFX;

    [Header("Jumping")]
    [SerializeField] private float maxJumpHeight = 4f;
    [SerializeField] private float minJumpHeight = 1f;
    [SerializeField] private float timeToJumpApex = 0.25f;
    private float maxJumpVelocity;
    private float minJumpVelocity;
    [SerializeField] private int maxJumps = 1;
    private int jumpCounter;
    [HideInInspector] public float lastPressedJump; // default value to avoid instant buffered jump
    [SerializeField] private float jumpBuffer = 0.1f;
    //private bool hasBufferedJump => lastPressedJump + jumpBuffer > Time.time;
    private bool hasBufferedJump => lastPressedJump + jumpBuffer > Time.time;

    [SerializeField] private float coyoteTime = 0.1f;
    private float lastGroundedTime;
    private bool canUseCoyoteTime => coyoteTimeActive && lastGroundedTime + coyoteTime > Time.time;
    private bool coyoteTimeActive = true;
    [SerializeField] private AudioClip jumpSFX;
    [SerializeField] private ParticleSystem jumpVFX;


    [Header("Dashing")]
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float dashDuration = 0.2f;
    private float dashVelocity;
    private float dashTimer;
    private float dashProgress = 0.5f;
    private float dashSmoothing;
    public int dashAmount = 1;
    private int dashCounter;
    private int dashingDirectionX;
    [HideInInspector] public float lastPressedDash; 
    [SerializeField] private float dashBuffer = 0.1f;
    private bool hasBufferedDash => lastPressedDash + dashBuffer > Time.time;
    [SerializeField] private AudioClip dashSFX;
    [SerializeField] private ParticleSystem dashVFX;

    [Header("Wall Jump")]
    [SerializeField] private Vector2 wallJumpClimb;
    [SerializeField] private Vector2 wallJumpOff;
    [SerializeField] private Vector2 wallLeap;
    [Header("Wall Sliding")]
    [SerializeField] private float wallSlideSpeedMax = 3.0f;
    [SerializeField] private float wallStickTime = 0.25f;
    private float timeToWallUnstick;
    private int wallDirectionX;

    [Header("Landing")]
    [SerializeField] private AudioClip landingSFX;
    [SerializeField] private ParticleSystem landingVFX;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        playerCollision = GetComponent<PlayerCollision>();
        animator = GetComponent<Animator>();
        audioManager = AudioManager.Instance;
    }

    private void Start()
    {
        playerInfo.Initialize();
        ResetJump();
        ResetDash();
        lastPressedJump = -0.2f; // to avoid instant buffered jump 
        lastPressedDash = -0.2f; // to avoid instant buffered dash

        gravity = -(2 * maxJumpHeight / Mathf.Pow(timeToJumpApex, 2));
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
    }
    private void FixedUpdate()
    {
        deltaTime = Time.fixedDeltaTime;
        UpdatePlayerInfo();
        BufferedInput();
        CalculateInputVelocity();
        WallSliding();
        Dashing();
        Gravity();

        // Hand over the input and calcualted velocity to the playercontroller handling the actual movement and collision
        Move(velocity);

        // Flip the players faceing direction if needed
        if (directionalInput.x > 0 && playerInfo.facingDirection < 0 )
        {
            FlipPlayer();
        }
        if (directionalInput.x < 0 && playerInfo.facingDirection > 0)
        {
            FlipPlayer();
        }

        // Movement animation and Feedback VFX & SFX
        if (playerCollision.collisionInfo.isGrounded && Mathf.Abs(velocity.x) > 0.1)
        {
            playerInfo.isRunning = true;
            animator.SetBool("IsRunning", true);
            runVFX.Play();
        }
        else
        {
            playerInfo.isRunning = false;
            animator.SetBool("IsRunning", false);
            runVFX.Stop();
        }
    }

    private void UpdatePlayerInfo() // could use clean up currently double book keeping in PlayerCollision and here
    {
        if (playerCollision.collisionInfo.isGrounded) 
        {
            lastGroundedTime = Time.time;
        }
        if (playerCollision.collisionInfo.isGrounded && !playerCollision.collisionInfo.wasGrounded) // check if player is landing
        {
            Landing();
        }
    }
    public void Landing()
    {
        velocity.y = 0;
        ResetJump();
        ResetDash();
        landingVFX.Play();
    }
    private void BufferedInput()
    {
        if (hasBufferedJump && !playerInfo.isWallsliding && !playerInfo.isJumping)
        {
            OnJumpInput();
        }
        if (hasBufferedDash && !playerInfo.isDashing)
        {
            OnDashInput();
        }
    }
    private void CalculateInputVelocity()
    {
        // Calculate the target X velocity based on input and movement speed. 
        float targetVelocityX = directionalInput.x * movementSpeed;

        //only control the player if grounded or airControl is turned on
        if (playerCollision.collisionInfo.isGrounded || playerInfo.hasAirControl)
        {
            // Reduce horizontal velocity when airborne
            var smoothedVelocityX = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (playerCollision.collisionInfo.isGrounded) ? accelerationTimeGrounded : accelerationTimeAirborn);
            velocity.x = (smoothedVelocityX < 0.01 && smoothedVelocityX > -0.01) ? 0 : smoothedVelocityX;

        }
    }
    private void Gravity()
    {
        if (!isGravity) return;
        if (playerCollision.collisionInfo.isGrounded) return;
        if (playerInfo.isWallsliding) return;
        if (playerInfo.isDashing) return;
        if (velocity.y + gravity * deltaTime < maxGravity) // smaller '<' than max. gravity because gravity is negative
        {
            velocity.y = maxGravity;
            return;
        }
        velocity.y += gravity * deltaTime;
    }
    public void Move(Vector2 aVelocity)
    {
        rb2D.velocity = aVelocity;
        rb2D.velocity += (playerCollision.collisionInfo.isStandingOnPlatform)? platformVelocity: Vector2.zero;
        rb2D.velocity += (playerInfo.isMagnetismActive)? magneticVelocity : Vector2.zero;
    }
    #region Wallsliding
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
    #endregion

    #region Jump
    private bool CanPlayerJump()
    {
        if (jumpCounter > 0) return true;
        if (canUseCoyoteTime) return true;
        return false;
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
            HandleJumping();
        }

         if (CanPlayerJump())
        {
            if (directionalInput.y! > -0.95f) // input values are from 0.999... to -0.999...
            {
                velocity.y = maxJumpVelocity;
            }
            HandleJumping();
        }
    }
    private void HandleJumping()
      {
        jumpCounter -= 1;
        playerInfo.isJumping = true;
        animator.SetBool("IsJumping", true);
        audioManager.PlaySound(jumpSFX, 0.25f);
    }
    public void OnJumpInputRelease()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }
    public void ResetJump()
    {
        jumpCounter = maxJumps;
        playerInfo.isJumping = false;
        coyoteTimeActive = true;
        animator.SetBool("IsJumping", false);
    }
    #endregion

    #region Dash
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
            audioManager.PlaySound(dashSFX, 0.5f);
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
            velocity.y = 0;
            velocity.x = dashVelocity * dashingDirectionX;

            if (dashTimer < (dashTimer * dashProgress))
            {
                float targetVelocityX = directionalInput.x * movementSpeed;
                velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref dashSmoothing, dashTimer * (1-dashProgress));
            }
            if(dashTimer < 0)
            {
                dashTimer = dashDuration;
                playerInfo.isDashing = false;

                if (playerCollision.collisionInfo.isGrounded)
                {
                    ResetDash();
                }
            }
            dashTimer -= deltaTime;
        }
    }
    public void ResetDash()
    {
        dashTimer = dashDuration;
        dashCounter = dashAmount;
        animator.SetBool("IsDashing", false);
    }
    #endregion
    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }
    void FlipPlayer()
    {
        playerInfo.facingDirection = (int)Mathf.Sign(directionalInput.x);

        //// Multiply the player's x local scale by -1.
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
    }
    public struct PlayerInfo
    {
        public int facingDirection;
        public bool isRunning { get; set; }
        public bool isWallsliding { get; set; }
        public bool isJumping { get; set; }
        public bool isDashing { get; set; }
        public bool hasAirControl { get; set; }
        public bool isMagnetismActive { get; set; }

        public void Initialize()
        {
            facingDirection = 1;
            isRunning = false;
            isWallsliding = false;
            isJumping = false;
            isDashing = false;
            hasAirControl = true;
            isMagnetismActive = false;
        }
    }
}
