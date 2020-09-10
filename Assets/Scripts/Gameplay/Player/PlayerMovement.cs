using UnityEngine;


/*
 * Basic set up of the script
 * https://youtu.be/MbWK8bCAU2w?list=PLFt_AvWsXl0f0hqURlhyIoAabKPgRsqjz
 */

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerAnimation))]
public class PlayerMovement : MonoBehaviour
{
    PlayerController playerController;
    PlayerAnimation playerAnimation;

    [HideInInspector] public PlayerInfo playerInfo;
    [HideInInspector] public Vector2 directionalInput;
    private Vector3 velocity;

    [SerializeField] private Animator animator;

    [Header("Movement")]
    public float movementSpeed = 6;
    private float velocityXSmoothing;
    private float accelerationTimeAirborn = 0.2f;
    private float accelerationTimeGrounded = 0.1f;
    public float maximumSlopeAngle = 60.0f;

    private float gravity;

    [Header("Jumping")]
    [SerializeField] private float maxJumpHeight = 4f;
    [SerializeField] private float minJumpHeight = 1;
    public float timeToJumpApex = 0.25f;

    private float maxJumpVelocity;
    private float minJumpVelocity;

    public int jumpAmount = 1;
    private int jumpCounter;


    [SerializeField] private float ghostJumpTime = 0.1f;
    private float ghostJumpTimer;
    private bool ghostJumpActive;

    [Header("Wall Jump")]
    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;
    [Header("Wall Sliding")]
    public float wallSlideSpeedMax = 3.0f;
    public float wallStickTime = 0.25f;
    private float timeToWallUnstick;
    private int wallDirectionX;


    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerController.maxSlopeAngle = maximumSlopeAngle;

        playerAnimation = GetComponent<PlayerAnimation>();

        gravity = -(2 * maxJumpHeight / Mathf.Pow(timeToJumpApex, 2));
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight); // https://www.youtube.com/watch?v=rVfR14UNNDo
        Debug.Log("Gravity: " + gravity + " Max. Jump Velocity: " + maxJumpVelocity + " Min. Jump Velocity:" + minJumpVelocity);

        playerInfo.Reset();
    }

    // Update is called once per frame
    private void Update()
    {
        // Calculate velocities 
        CalculateVelocity();
        WallSliding();
        UpdateJump();

        // Hand over the input and calcualted velocity to the playercontroller handling the actual movement and collision
        playerController.Move(velocity * Time.deltaTime, directionalInput);

        // Reset gravity to 0 to avoid gravity amassing when simply standing around. We want to do this after we have moved the player with our input because platforms also move the player
        if (playerController.collisionInfo.above || playerController.collisionInfo.below)
        {
            if (playerController.collisionInfo.slidingDownMaxSlope)
            {
                velocity.y += playerController.collisionInfo.slopeNormal.y * -gravity * Time.deltaTime; // oppose velocity y by gravity over time 
            }
            else
            {
                velocity.y = 0;
            }
        }
    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    private void UpdateJump()
    {
        if (playerController.collisionInfo.below)
        {
            jumpCounter = 0;
            ghostJumpActive = false;
            ghostJumpTimer = ghostJumpTime;

        }

        if (ghostJumpTimer > 0 && ghostJumpActive)
        {
            ghostJumpTimer -= Time.deltaTime;
        }

        if (playerController.collisionInfo.lastFrameBelow == true && playerController.collisionInfo.below == false)
        {
            ghostJumpActive = true;
        }

        if (ghostJumpTimer < 0)
        {
            ghostJumpActive = false;
            animator.SetBool("IsJumping", false); // this is SHIT needs rework should be called when player controller detects that it is grounded 
        }
    }

    private void CanPlayerJump()
    {
        playerInfo.canJump = false;
        if (jumpCounter < jumpAmount)
        {
            if (playerController.collisionInfo.below || playerInfo.isWallsliding)
            {
                playerInfo.canJump = true;
            }

            if (ghostJumpActive)
            {
                playerInfo.canJump = true;
            }
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

        // If we stand on smth. and doesn't press down we set maxJumpvelocity
        if (playerInfo.canJump)
        {
            if (playerController.collisionInfo.slidingDownMaxSlope)
            {
                if (directionalInput.x != -Mathf.Sign(playerController.collisionInfo.slopeNormal.x)) // if we are not jumping against max slope normal
                {
                    velocity.y = maxJumpVelocity * playerController.collisionInfo.slopeNormal.y;
                    velocity.x = maxJumpVelocity * playerController.collisionInfo.slopeNormal.x;

                }
            }

            else if (directionalInput.y != -1)
            {
                velocity.y = maxJumpVelocity;
            }

            jumpCounter += 1;
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



    private void WallSliding()
    {
        // Figure out wall direction
        wallDirectionX = (playerController.collisionInfo.left) ? -1 : 1;


        playerInfo.isWallsliding = false;
        if ((playerController.collisionInfo.left || playerController.collisionInfo.right) && !playerController.collisionInfo.below && velocity.y < 0)
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
                    timeToWallUnstick -= Time.deltaTime;
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

    public void Dash()
    {
        Debug.Log("Dash");
    }

    private void CalculateVelocity()
    {
        // Calculate the target X velocity based on input and movement speed. 
        float targetVelocityX = directionalInput.x * movementSpeed;

        // Reduce horizontal velocity when airborne
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (playerController.collisionInfo.below) ? accelerationTimeGrounded : accelerationTimeAirborn);

        // Manipulate gravity slightly when falling to get more weight feel or give player more time to fix mistakes
        velocity.y += gravity * Time.deltaTime;
    }

    public struct PlayerInfo
    {
        public bool alive, dead;
        public bool isWallsliding;
        public bool canJump;
        public bool canDash;

        public void Reset()
        {
            alive = true;
            dead = false;
            isWallsliding = false;
            canJump = true;
            canDash = true;
        }
    }
}
