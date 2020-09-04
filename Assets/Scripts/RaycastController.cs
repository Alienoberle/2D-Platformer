using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Basic set up of the script
 * https://youtu.be/MbWK8bCAU2w?list=PLFt_AvWsXl0f0hqURlhyIoAabKPgRsqjz
 * https://youtu.be/z20wHJSXk98?t=901 
 */

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour
{
    public LayerMask collisionMask;

    [HideInInspector]
    public new BoxCollider2D collider;

    public RaycastOrigins raycastOrigins;
    public const float skinWidth = 0.015f;
    const float distanceBetweenRays = 0.2f;
    [HideInInspector]
    public int horizontalRayCount;
    [HideInInspector]
    public int verticalRayCount;
        
    [HideInInspector]
    public float horizontalRaySpacing;
    [HideInInspector]
    public float verticalRaySpacing;

    
    public virtual void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
    }
    public virtual void Start()
    {
        CalculateRaySpacing(); // only needs to be called once or when the ray count changes
    }

    public void UpdateRaycastOrigins()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    public void CalculateRaySpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        float boundsWidth = bounds.size.x;
        float boundsHeight = bounds.size.y;

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, Mathf.RoundToInt(boundsHeight / distanceBetweenRays), int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, Mathf.RoundToInt(boundsWidth / distanceBetweenRays), int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
}
