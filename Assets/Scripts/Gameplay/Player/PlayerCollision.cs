using UnityEngine;

// How to set up playerController and extract Raycast class
// https://www.youtube.com/watch?v=MbWK8bCAU2w

public class PlayerCollision : RaycastController
{
    [SerializeField] private LayerMask collisionMask;
    public CollisionInfo collisionInfo;
    [SerializeField] private PlayerController playerController;
    [HideInInspector] public float maxSlopeAngle;
    

    public override void Awake()
    {
        base.Awake();
    }
    public override void Start()
    {
        base.Start();
    }

    public void FixedUpdate()
    {
        UpdateRaycastOrigins();
        collisionInfo.Reset();
        HorizontalCollisions(playerController.rb2D.velocity * Time.fixedDeltaTime);
        VerticalCollisions(playerController.rb2D.velocity * Time.fixedDeltaTime);
    }
    void HorizontalCollisions(Vector2 moveAmount)
    {
        float directionX = Mathf.Sign(moveAmount.x); // direction +1 or -1 of the x moveAmount
        float rayLenght = Mathf.Abs(moveAmount.x) + skinWidth; // get the absolute lenght which is always positive and add the skinwidth
        if (Mathf.Abs(moveAmount.x) < skinWidth)
        {
            rayLenght = skinWidth * 2; // have minimal raylenght to detect the wall if standing still
        }
        if (moveAmount.x != 0)
        {
            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * horizontalRaySpacing * i;
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLenght, collisionMask);
                Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLenght, Color.red);

                // directly continue with the next ray if the player is within a collision
                if (hit.distance == 0)
                {
                    continue;
                }
                if (hit)
                {
                    // calculate the angel between the normal vector or the slope hit and the up vector which is the same as the angle of the slope
                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                    var hitLayer = hit.transform.gameObject.layer;
                    collisionInfo.left = directionX == -1; // if we hit smth. and we are going left we set collision info 'left'
                    collisionInfo.right = directionX == 1; // if we hit smth. and are going right we set collisin info 'right'
                }
            }
        }
        else // check left and right in case the player is wallsliding or not moving on his own
        {
            for (int i = 0; i < horizontalRayCount; i++)  // check left side
            {
                Vector2 rayOrigin = raycastOrigins.bottomLeft;
                rayOrigin += Vector2.up * horizontalRaySpacing * i;
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * -1, rayLenght, collisionMask);
                Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLenght, Color.red);

                if (hit)
                {
                    // calculate the angel between the normal vector or the slope hit and the up vector which is the same as the angle of the slope
                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                    var hitLayer = hit.transform.gameObject.layer;
                    collisionInfo.left = true; // if we hit smth. and we are going left we set collision info 'left'
                }
            }
            for (int i = 0; i < horizontalRayCount; i++)  // check right side
            {
                Vector2 rayOrigin = raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * horizontalRaySpacing * i;
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * 1, rayLenght, collisionMask);
                Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLenght, Color.red);

                if (hit)
                {
                    // calculate the angel between the normal vector or the slope hit and the up vector which is the same as the angle of the slope
                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                    var hitLayer = hit.transform.gameObject.layer;
                    collisionInfo.right = true; // if we hit smth. and are going right we set collisin info 'right'
                }
            }
        }
    }
    void VerticalCollisions(Vector2 moveAmount)
    {
        float directionY = (moveAmount.y == 0 )? -1: Mathf.Sign(moveAmount.y); // direction +1 or -1 of the y moveAmount, choose to detect ground if no vertical movement
        float rayLenght = Mathf.Abs(moveAmount.y) + skinWidth; // get the absolute lenght which is always positive and add the inset
        if (Mathf.Abs(moveAmount.y) < skinWidth)
        {
            rayLenght = skinWidth * 2; // have minimal raylenght to detect the floor if standing still
        }
        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x * Time.deltaTime); // + moveAmount x to also check the place we will be after moving
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLenght, collisionMask);
            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLenght, Color.red);

            // directly continue with the next ray if the player is within a collision
            if (hit.distance == 0)
            {
                continue;
            }
            if (hit)
            {
                if(directionY == -1) // if we hit smth. and we are going downwards we set collision info 'below'
                {
                    collisionInfo.below = true;
                    var hitLayer = hit.transform.gameObject.layer;
                    switch (hitLayer) //can check for more layers if nescessary
                    {
                        case 9:
                            collisionInfo.isGrounded = true;
                            break;
                        case 10:
                            collisionInfo.isGrounded = true;
                            collisionInfo.isStandingOnPlatform = true;
                            break;
                        case 11:
                            collisionInfo.isGrounded = true;
                            break;
                    }
                }
                else
                {
                    collisionInfo.below = false;
                    collisionInfo.isGrounded = false;
                }
                collisionInfo.above = directionY == 1; // if we hit smth. and are going upwards we set collision info 'above'
            }
        }
    }
    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;
        public bool isGrounded, wasGrounded;
        public bool isStandingOnPlatform;
        public void Reset()
        {
            above = below = false;
            left = right = false;
            wasGrounded = isGrounded;
            isGrounded = false;
            isStandingOnPlatform = false;
        }
    }
}


