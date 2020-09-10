using UnityEngine;

// How to set up playerController and extract Raycast class
// https://www.youtube.com/watch?v=MbWK8bCAU2w

public class PlayerController : RaycastController
{
    public CollisionInfo collisionInfo;

    [HideInInspector]
    public Vector2 playerInput;
    private Vector2 initialVelocity;

    [HideInInspector]
    public float maxSlopeAngle;

    [HideInInspector]
    public bool playerPressedDown;

    public override void Start()
    {
        base.Start();

        // just to give a starting direction
        collisionInfo.faceingDirection = 1;

    }

    // Move overload Method that just calls the move method but with zero input in case of moving platforms
    public void Move(Vector2 moveAmount, bool standingOnPlatform)
    {
        Move(moveAmount, Vector2.zero, standingOnPlatform);
    }

    public void Move(Vector2 moveAmount, Vector2 playerInput, bool standingOnPlatform = false)
    {
        UpdateRaycastOrigins();
        collisionInfo.Reset();
        initialVelocity = moveAmount;

        // store the input
        this.playerInput = playerInput;

        if (moveAmount.y < 0)
        {
            DescendSlope(ref moveAmount);
        }

        // determine the facing direction and store it for later use. do it after descend slope as this may change (force) direction
        if (moveAmount.x != 0)
        {
            collisionInfo.faceingDirection = (int)Mathf.Sign(moveAmount.x);
        }

        HorizontalCollisions(ref moveAmount);

        if (moveAmount.y != 0)
        {
            VerticalCollisions(ref moveAmount);
        }

        transform.Translate(moveAmount);

        if (standingOnPlatform)
        {
            collisionInfo.below = true;
        }
    }

    void HorizontalCollisions(ref Vector2 moveAmount)
    {
        float directionX = collisionInfo.faceingDirection; // direction +1 or -1 of the y moveAmount
        float rayLenght = Mathf.Abs(moveAmount.x) + skinWidth; // get the absolute lenght which is always positive and add the skinwidth

        if (Mathf.Abs(moveAmount.x) < skinWidth)
        {
            rayLenght = skinWidth * 2; // have minimal raylenght to detect the wall if standing still
        }

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * horizontalRaySpacing * i;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLenght, collisionMask);
            Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

            // directly continue with the next ray if the player is within a collision
            if (hit.distance == 0)
            {
                continue;
            }

            if (hit)
            {
                // calculate the angel between the normal vector or the slope hit and the up vector which is the same as the angle of the slope
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                // we only want to check for slopes for the bottom most horizontal raycast, others could interfere
                if (i == 0 && slopeAngle <= maxSlopeAngle)
                {
                    // if we notice we climb a slope we reset moveAmount to initial moveAmount in case we were descending a slope before
                    if (collisionInfo.descendingSlope)
                    {
                        collisionInfo.descendingSlope = false;
                        moveAmount = initialVelocity;
                    }

                    float distanceToSlopeStart = 0;
                    if (slopeAngle != collisionInfo.slopeAngleOld) // only when new slope
                    {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        moveAmount.x -= distanceToSlopeStart * directionX; // reduce the X moveAmount by the distance to slope so it only starts climibing when actually having reached the slope
                    }

                    ClimbSlope(ref moveAmount, slopeAngle, hit.normal); // actually climb slope 
                    moveAmount.x += distanceToSlopeStart * directionX;
                }

                if (!collisionInfo.climbingSlope || slopeAngle > maxSlopeAngle) // when climbing the slope we don't want to check collisions for the rest of the rays 
                {
                    moveAmount.x = (hit.distance - skinWidth) * directionX; // get the distance to collision - the skinwidth added to the rayLenght before 
                    rayLenght = hit.distance; // set to make sure the smallest ray is considered to not move trough objects

                    // to avoid bouncing behavoir when encountering horizontal collision when climbing slopes, we also have to update Y moveAmount 
                    if (collisionInfo.climbingSlope)
                    {
                        moveAmount.y = Mathf.Tan(collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
                    }

                    collisionInfo.left = directionX == -1; // if we hit smth. and we are going left we set collision info 'left'
                    collisionInfo.right = directionX == 1; // if we hit smth. and are going right we set collisin info 'right'
                }
            }
        }
    }

    // speed should while climbing should be the same as whe movin normally. So velocityX will be the targetdistance we want to move up the slope. 
    // based on that targetdistance and the slopeAngle we want to figure out what our velocities x & y have to be.
    private void ClimbSlope(ref Vector2 moveAmount, float slopeAngle, Vector2 slopeNormal)
    {
        float moveDistance = Mathf.Abs(moveAmount.x); // get the positive distance
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (moveAmount.y <= climbVelocityY) // jump y moveAmount is greater than climb moveAmount
        {
            moveAmount.y = climbVelocityY;
            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x); // we want to keep the direction left = -1,  right = 1

            collisionInfo.climbingSlope = true;
            collisionInfo.slopeAngle = slopeAngle;
            collisionInfo.slopeNormal = slopeNormal;
            collisionInfo.below = true; // since we are climbing we can safely assume that we are on the ground and have collision below us

        }
    }

    void VerticalCollisions(ref Vector2 moveAmount)
    {
        float directionY = Mathf.Sign(moveAmount.y); // direction +1 or -1 of the y moveAmount
        float rayLenght = Mathf.Abs(moveAmount.y) + skinWidth; // get the absolute lenght which is always positive and add the inset

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x); // + moveAmount x to also check the place we will be after moving
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLenght, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);

            if (hit)
            {
                // check if the collision has the move trough tag 
                if (hit.collider.tag == "MoveThrough")
                {
                    // and we are moving up (jumping through) we just continue with the next ray
                    if (directionY == 1 || hit.distance == 0)
                    {
                        continue;
                    }

                    if (collisionInfo.fallingTroughPlatform)
                    {
                        continue;
                    }

                    // and we want to move down (fall trough) we also just continue with the next ray
                    if (playerInput.y == -1 && playerPressedDown)
                    {
                        collisionInfo.fallingTroughPlatform = true;
                        Invoke("ResetFallingTroughPlatform", 0.25f);
                        continue;
                    }

                }

                moveAmount.y = (hit.distance - skinWidth) * directionY; // get the distance to collision - the skinwidth added to the rayLenght before 
                rayLenght = hit.distance; // set to make sure the ray without skin is considered to not move trough objects

                // If we climb and collide with smth. above use we need to also calculate the x moveAmount accordingly to avoid bouncing
                if (collisionInfo.climbingSlope)
                {
                    moveAmount.x = moveAmount.y / Mathf.Tan(collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmount.x);
                }

                collisionInfo.below = directionY == -1; // if we hit smth. and we are going downwards we set collision info 'below'
                collisionInfo.above = directionY == 1; // if we hit smth. and are going upwards we set collisin info 'above'
            }
        }

        // when climbing a slope fire a horizontal ray on the height where we will be once we move up the slope to check if there is a new slope 
        if (collisionInfo.climbingSlope)
        {
            float directionX = Mathf.Sign(moveAmount.x);
            float rayLenghtX = Mathf.Abs(moveAmount.x) + skinWidth;

            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + (Vector2.up * moveAmount.y);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLenghtX, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLenghtX, Color.red);

            if (hit)
            {
                float newSlopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                Debug.Log(newSlopeAngle);
                if (newSlopeAngle != collisionInfo.slopeAngle)
                {
                    moveAmount.x = (hit.distance - skinWidth) * directionX;
                    collisionInfo.slopeAngle = newSlopeAngle;
                    collisionInfo.slopeNormal = hit.normal;
                    collisionInfo.below = true; // since we are climbing we can safely assume that we are on the ground and have collision below us
                }
            }
        }
    }


    private void DescendSlope(ref Vector2 moveAmount)
    {
        RaycastHit2D maxSlopeLeft = Physics2D.Raycast(raycastOrigins.bottomLeft, Vector2.down, Mathf.Abs(moveAmount.y) + skinWidth, collisionMask);
        RaycastHit2D maxSlopeRight = Physics2D.Raycast(raycastOrigins.bottomRight, Vector2.down, Mathf.Abs(moveAmount.y) + skinWidth, collisionMask);

        // we only move on to sliding down, if only one of the side detects a hit and other is "up in air" to avoid jitter with super flat slopes
        if (maxSlopeLeft ^ maxSlopeRight)
        {
            SlideDownMaxSlope(maxSlopeLeft, ref moveAmount);
            SlideDownMaxSlope(maxSlopeRight, ref moveAmount);
        }

        if (!collisionInfo.slidingDownMaxSlope)
        {
            float directionX = Mathf.Sign(moveAmount.x);
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, collisionMask);
            Debug.DrawRay(rayOrigin, Vector2.down, Color.magenta);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle)
                {
                    // check which direction the slope is facing and compare that to the players movement direction and if they are, we are moving down the slope
                    if (Mathf.Sign(hit.normal.x) == directionX)
                    {
                        // want to check if we are close enough for the slope to actually take effect
                        float distanceToSlope = hit.distance - skinWidth;
                        if (distanceToSlope <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x)) // distance to slope shorter than what we would have to move on the y axis based on angle and x moveAmount
                        {
                            float moveDistance = Mathf.Abs(moveAmount.x); // get the positive distance 
                            float descendVelocity = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

                            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                            moveAmount.y -= descendVelocity;

                            collisionInfo.slopeAngle = slopeAngle;
                            collisionInfo.descendingSlope = true;
                            collisionInfo.slopeNormal = hit.normal;
                            collisionInfo.below = true; // since we are descending a slope we can safely assume that we are on the ground and have collision below us
                        }
                    }
                }
            }
        }

    }

    void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 moveAmount)
    {
        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            float slopeDirection = hit.normal.x;

            if (slopeAngle > maxSlopeAngle)
            {
                moveAmount.x = slopeDirection * (Mathf.Abs(moveAmount.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad);

                collisionInfo.slopeAngle = slopeAngle;
                collisionInfo.slidingDownMaxSlope = true;
                collisionInfo.slopeNormal = hit.normal;
                collisionInfo.below = true; // since we are descending a slope we can safely assume that we are on the ground and have collision below us

            }
        }
    }

    void ResetFallingTroughPlatform()
    {
        collisionInfo.fallingTroughPlatform = false;

    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public bool lastFrameBelow;

        public bool climbingSlope;
        public bool descendingSlope;
        public bool slidingDownMaxSlope;

        public float slopeAngle;
        public float slopeAngleOld;
        public Vector2 slopeNormal;

        public int faceingDirection;

        public bool fallingTroughPlatform;

        public Vector2 initialVelocity;

        public void Reset()
        {
            lastFrameBelow = below;

            above = below = false;
            left = right = false;
            climbingSlope = false;
            descendingSlope = false;
            slidingDownMaxSlope = false;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
            slopeNormal = Vector2.zero;
        }
    }
}


