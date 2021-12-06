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

    public void Update()
    {
        UpdateRaycastOrigins();
        collisionInfo.Reset();
        HorizontalCollisions(playerController.rb2D.velocity * Time.fixedDeltaTime);
        VerticalCollisions(playerController.rb2D.velocity * Time.fixedDeltaTime);
    }

    void HorizontalCollisions(Vector2 moveAmount)
    {
        float directionX = playerController.playerInfo.facingDirection; // direction +1 or -1 of the x moveAmount
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
            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLenght, Color.red);

            //Deactivated for now as it is not needed 
            //if (hit)
            //{
            //    var hitLayer = hit.transform.gameObject.layer;
            //    switch (hitLayer) //can chek for more layers if nescessary
            //    {
            //        case 12: 
            //            Debug.Log("Wall");
            //            break;
            //    }
            //}
        }
    }

    void VerticalCollisions(Vector2 moveAmount)
    {
        float directionY = (moveAmount.y == 0 )? -1: Mathf.Sign(moveAmount.y); // direction +1 or -1 of the y moveAmount, choose to detect ground if no vertical movement
        float rayLenght = Mathf.Abs(moveAmount.y) + skinWidth; // get the absolute lenght which is always positive and add the inset
        if (Mathf.Abs(moveAmount.y) < skinWidth)
        {
            rayLenght = skinWidth * 2; // have minimal raylenght to detect the wall if standing still
        }
        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x * Time.deltaTime); // + moveAmount x to also check the place we will be after moving
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLenght, collisionMask);
            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLenght, Color.red);
            
            if (hit)
            {
                collisionInfo.isGrounded = true;
                var hitLayer = hit.transform.gameObject.layer;
                switch (hitLayer) //can chek for more layers if nescessary
                {
                    case 10:
                        Debug.Log("Platform");
                        collisionInfo.isStandingOnPlatform = true;
                        break;
                }
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


