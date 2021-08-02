using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private PlayerInputHandler playerInput;
    public PlayerController player;
    private PlayerCollision target;

    FocusArea focusArea;

    [Tooltip("Defines the size of the focus area.")]
    public Vector2 focusAreaSize = new Vector2(2.5f, 4.5f);
    [Tooltip("Disable if you want to turn off the focus area in the editor.")]
    public bool drawFocusArea = true;

    [Tooltip("The offset of the camera on the y axis.")]
    public float verticalOffset = 1.0f;
    [Tooltip("How much the camera should look ahead the player when moving horizontally.")]
    public float lookAheadDistanceX = 2.0f;
    [Tooltip("Time if takes the camera to smoothly move horizontally. The longer the smoother and sluggisher the camera moves.")]
    public float lookSmoothTimeX = .75f;
    [Tooltip("Time if takes the camera to smoothly move vertically. The longer the smoother and sluggisher the camera moves. With high jump or falls the smoothtime may have to be turned off.")]
    public float lookSmoothTimeY = .1f;

    float currentLookAheadX;
    float targetLookAheadX;
    float lookAheadDirectionX;
    float smoothLookVelocityX;
    float smoothLookVelocityY;

    bool lookAheadStopped;

    void Start()
    {
        player = Player.instance.GetComponent<PlayerController>();

        // Grab the needed components from the player
        target = player.GetComponent<PlayerCollision>();
        playerInput = player.GetComponent<PlayerInputHandler>();

        // Make we have a focus area size at least the size of the player bounds
        if (focusAreaSize.x < target.collider.bounds.size.x || focusAreaSize.x < target.collider.bounds.size.y)
        {
            focusAreaSize.x = target.collider.bounds.size.x;
            focusAreaSize.y = target.collider.bounds.size.y;
            Debug.LogWarning("Camera Focus Area is not set up correctly! " + "Set Focus Area Size x to: " + focusAreaSize.x + " Set Focus Area Size y to: " + focusAreaSize.y);
        }

        focusArea = new FocusArea(target.collider.bounds, focusAreaSize);
    }

    // We want to move the camera after all the player movement for this frame has already happend
    void LateUpdate()
    {
        focusArea.Update(target.collider.bounds);

        Vector2 focusPosition = focusArea.center + Vector2.up * verticalOffset;

        if (focusArea.focusMovementLastFrame.x != 0)
        {
            // Figure out which direction the focus direction has move last frame
            lookAheadDirectionX = Mathf.Sign(focusArea.focusMovementLastFrame.x);

            // We move it the full look ahead distance if it moved in the same direction as the player input is if it was not 0
            // this avoids issues with the smoothing applied to the player itself (in the player controller)
            if (Mathf.Sign(playerInput.directionalInput.x) == Mathf.Sign(focusArea.focusMovementLastFrame.x) && playerInput.directionalInput.x != 0)
            {
                lookAheadStopped = false;
                targetLookAheadX = lookAheadDirectionX * lookAheadDistanceX;
            }
            else
            {
                if (!lookAheadStopped)
                {
                    lookAheadStopped = true;
                    // We only add a fraction of the remaining distance to look ahead
                    targetLookAheadX = currentLookAheadX + ((lookAheadDirectionX * lookAheadDistanceX - currentLookAheadX) / 4f);
                }
            }
        }

        // smooth the horizonal camera movement
        currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);
        // smooth the vertical camera movement
        focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothLookVelocityY, lookSmoothTimeY);

        focusPosition += Vector2.right * currentLookAheadX;
        transform.position = (Vector3)focusPosition + Vector3.forward * -10;
    }

    void OnDrawGizmos()
    {
        if (drawFocusArea)
        {
            Gizmos.color = new Color(1, 0, 0, 0.25f);
            Gizmos.DrawCube(focusArea.center, focusAreaSize);
        }
    }

    struct FocusArea
    {
        public Vector2 center;
        public Vector2 focusMovementLastFrame;

        float left, right;
        float top, bottom;


        public FocusArea(Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;

            focusMovementLastFrame = Vector2.zero;
            center = new Vector2((left + right) / 2, (top + bottom) / 2);
        }
        public void Update(Bounds targetBounds)
        {
            float shiftX = 0;
            if (targetBounds.min.x < left)
            {
                shiftX = targetBounds.min.x - left;
            }
            else if (targetBounds.max.x > right)
            {
                shiftX = targetBounds.max.x - right;
            }
            left += shiftX;
            right += shiftX;

            float shiftY = 0;
            if (targetBounds.min.y < bottom)
            {
                shiftY = targetBounds.min.y - bottom;
            }
            else if (targetBounds.max.y > top)
            {
                shiftY = targetBounds.max.y - top;
            }
            bottom += shiftY;
            top += shiftY;
            center = new Vector2((left + right) / 2, (top + bottom) / 2);

            // Store how far the focus area has move in the last frame
            focusMovementLastFrame = new Vector2(shiftX, shiftY);
        }

    }

}
